using System.Collections.Generic;
using System.Linq;
using Engine.FeatureExtraction;
using FeatureExtraction;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;

namespace Engine.Kinect
{
    /// <summary>
    /// Process and Keep track of the entropy over time, and decide if the entropy
    ///   drops below critical level (to indicate silence).
    /// </summary>
    public class EntropyMonitor
    {
        private const double EntropyLimit = 0.035;
        private readonly int framesPerSecond;

        public EntropyMonitor(int framesPerSecond)
        {
            this.framesPerSecond = framesPerSecond;
        }

        /// <summary>
        /// A data structure used in the aggregation of entropy over multiple frames
        /// </summary>
        private class EntropyAcc
        {
            public static readonly EntropyAcc Zero = new EntropyAcc(null, 0, 0);
            private readonly List<Vector3D> lastFrame;
            private readonly int intervalCount;
            public readonly double CumulativeEntropy;

            private EntropyAcc(List<Vector3D> frame, double entropy, int count)
            {
                lastFrame = frame;
                CumulativeEntropy = entropy;
                intervalCount = count;
            }

            public static EntropyAcc Aggregate(EntropyAcc last, List<Vector3D> cur)
            {
                return last.lastFrame == null ? new EntropyAcc(cur, 0, 0) : 
                    new EntropyAcc(cur, last.CumulativeEntropy + SquaredDist(last.lastFrame, cur), last.intervalCount+1);
            }
        }

        /// <summary>
        /// Find the Cumulative Entropy over the last second worth of frames
        /// </summary>
        /// <param name="frameHistory">A stack of frames (i.e in reverse chronological order)</param>
        /// <returns>The cumulative entropy over the last second</returns>
        public double CalculateEntropy(Stack<List<Vector3D>> frameHistory)
        {
            // choose only the last second worth of frames
            return frameHistory.Take(framesPerSecond+1).Aggregate(EntropyAcc.Zero, EntropyAcc.Aggregate).CumulativeEntropy;
        }

        private static double SquaredDist(IEnumerable<Vector3D> cur, IEnumerable<Vector3D> old)
        {
            return cur.Zip(old, SquaredDist).Sum();
        }

        private static double SquaredDist(Vector3D cur, Vector3D old)
        {
            double dx = cur.X - old.X;
            double dy = cur.Y - old.Y;
            double dz = cur.Z - old.Z;

            return dx*dx + dy*dy + dz*dz;
        }

        public bool IsLowEntropy(Stack<List<Vector3D>> frameHistory)
        {
            return CalculateEntropy(frameHistory) <= EntropyLimit;
        }

        /// <summary>
        /// Convert a Skeleton into a list of vectors, by filtering out only the 
        ///   most relevant body parts (for calculating movement).
        /// </summary>
        /// <param name="skel">A processed skeleton</param>
        /// <returns>A list containing the most relevant body parts' positions</returns>
        public static List<Vector3D> ToVectorList(Skeleton skel)
        {
            // only consider the 4 key parts :  { Elbow, Wrist } x { Left, Right }
            return new List<Vector3D> { skel.ElbowLeft, skel.WristLeft, skel.ElbowRight, skel.WristRight };
        }
    }
}