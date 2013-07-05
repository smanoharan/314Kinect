using System;
using System.Collections;
using Engine.Kinect;
using FeatureExtraction;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;
using Microsoft.Research.Kinect.Nui;

namespace Engine.FeatureExtraction
{
    public class DepthMapPreprocessor : IPreprocessor
    {
        private const int VideoWidth = 640;
        private const int VideoHeight = 480;
        private const int VideoCropSize = 64;
        private const int VideoPixelBytes = 4;

        private const int DepthWidth = 320;
        private const int DepthHeight = 240;
        private const int DepthCropSizeHalf = 32;
        private const int DepthCropSize = 64;
        private const int DepthPixelBytes = 2;
        private const int DepthThreshold = 100;

        private static void HandCoords(Vector3D handPos, out int depthX, out int depthY)
        {
            float depthXf, depthYf;
            KinectHandler.Get().GetDepthCoordsFromSkeleton(handPos, out depthXf, out depthYf);

            // convert to integer co-ordinates (from (1 x 1) space to (width x height)):
            depthX = (int)Math.Max(0, Math.Min(depthXf * DepthWidth, DepthWidth));
            depthY = (int)Math.Max(0, Math.Min(depthYf * DepthHeight, DepthHeight));
        }

        private static void HandCoords(Vector3D handPos, out int videoX, out int videoY, out int depthX, out int depthY)
        {
            // first get depth values, then video:
            HandCoords(handPos, out depthX, out depthY);
            KinectHandler.Get().GetVideoCoordsFromDepth(ImageResolution.Resolution640x480, depthX, depthY, out videoX, out videoY);
        }

        public virtual PreprocessedFrame Preprocess(Skeleton skel, byte[] video, byte[] depth)
        {
            // process the hands, left then right:
            var left = ProcessHand(skel.PalmLeft, skel.WristLeft, video, depth);
            var right = ProcessHand(skel.PalmRight, skel.WristRight, video, depth);

            //var features = SkeletonFeatureFrame.SkeletonDataToFeatureVector(skel);
            var features = SkeletonDataAdapter.FeatureSet(skel);
            
            return new PreprocessedFrame(features, left.Item1, right.Item1, left.Item2, right.Item2);
        }

        private static Tuple<byte[], BitArray> ProcessHand(Vector3D handPos, Vector3D wristPos, byte[] video, byte[] depth)
        {
            // convert hand and wrist positions to respective co-ord systems:
            int depthHandX, depthHandY, depthWristX, depthWristY, videoX, videoY;
            HandCoords(wristPos, out depthWristX, out depthWristY);
            HandCoords(handPos, out videoX, out videoY, out depthHandX, out depthHandY);

            // extract depth values
            return Tuple.Create(CropVideo(video, videoX, videoY),
                GetDepthMap(depth, depthHandX, depthHandY, depthWristX, depthWristY));
        }

        private static byte[] CropVideo(byte[] origVideo, int handX, int handY)
        {
            return CropUtil.CropImageToHandPosition(handX, handY, VideoCropSize, origVideo, VideoHeight,
                VideoWidth, VideoPixelBytes);
        }

        private static int ToDepth(byte b1, byte b2)
        {
            return (b1 >> 3 | b2 << 5);
        }

        private static BitArray GetDepthMap(byte[] depth, int handX, int handY, int wristX, int wristY)
        {
            // get inequality for filtering pixels past hand-wrist line
            Inequality2D ineq = Inequality2D.CreateFromHandWrist(wristX, wristY, handX, handY);

            // crop the depth image to the hand
            int startX, startY;
            var croppedDepth = CropUtil.CropImageToHandPosition(handX, handY, DepthCropSizeHalf, depth,
                DepthHeight, DepthWidth, DepthPixelBytes, out startX, out startY);


            // build up the bitArray for filtering past hand-wrist line.
            // keep track of minimum depth encountered.
            int minDepth = 10000;
            var depthMap = new int[croppedDepth.Length / 2];
            for (int x = 0; x < DepthCropSize; x++)
            {
                for (int y = 0; y < DepthCropSize; y++)
                {
                    int index = x + (y * DepthCropSize);
                    int curDepth = ToDepth(croppedDepth[index * 2], croppedDepth[index * 2 + 1]);

                    // need curDepth > 750 to negate impact of (partial) occlusion.
                    // Kinect shouldn't give any results shorter than 800mm = 0.8m
                    if (curDepth > 750 && curDepth < minDepth) minDepth = curDepth;

                    depthMap[index] = ineq.IsAllowed(x + startX, y + startY) ? curDepth : 0;
                }
            }

            // threshold by depth
            BitArray depthBitMap = new BitArray(depthMap.Length);
            for (int i = 0; i < depthMap.Length; i++)
            {
                if (Math.Abs(depthMap[i] - minDepth) < DepthThreshold)
                    depthBitMap[i] = true;
            }

            return depthBitMap;
        }
    }

    // inequality of the form y < mx + c or y > mx + c
    public class Inequality2D
    {
        private readonly double m;
        private readonly double c;
        private readonly bool isDiffPositive; // is (ActualY - LineY) positive?

        protected Inequality2D(double m, double c, bool isDiffPositive)
        {
            this.m = m;
            this.c = c;
            this.isDiffPositive = isDiffPositive;
        }

        protected virtual double GetYDifference(int x, int y)
        {
            return y - (x * m + c);
        }

        public bool IsAllowed(int x, int y)
        {
            double yDiff = GetYDifference(x, y);
            return Math.Abs(yDiff - 1) < 0 || isDiffPositive == (yDiff > 0);
        }

        public static Inequality2D CreateFromHandWrist(int wristX, int wristY, int handX, int handY)
        {
            // find gradient of the hand-wrist line
            int dyLine = (handY - wristY);
            int dxLine = (handX - wristX);

            if (dyLine == 0) // line is horizontal, so normal is vertical;
                return new VerticalInequality2D(wristX, dxLine > 0);

            double m = -(dxLine / (double)dyLine);
            double c = wristY - m * wristX;
            bool isDiffPositive = handY - (handX * m + c) > 0;
            return new Inequality2D(m, c, isDiffPositive);
        }
    }

    public class VerticalInequality2D : Inequality2D
    {
        private readonly double xValue;
        public VerticalInequality2D(double xValue, bool isDiffPositive)
            : base(0, 0, isDiffPositive)
        {
            this.xValue = xValue;
        }

        protected override double GetYDifference(int x, int y)
        {
            return x - xValue;
        }
    }
}
