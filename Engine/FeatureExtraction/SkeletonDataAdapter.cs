using System;
using FeatureExtraction;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;
using Microsoft.Research.Kinect.Nui;

namespace Engine.FeatureExtraction
{
    /// <summary>
    /// A set of helper methods for interfacing the Skeleton Data structures and the Kinect.
    /// </summary>
    public static class SkeletonDataAdapter
    {
        private static Vector3D To3DVector(this SkeletonData data, JointID joint)
        {
            var v = data.Joints[joint].Position;
            return new Vector3D(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Convert a Kinect SkeletonData into the Processed Skeleton data structure.
        /// </summary>
        /// <param name="data">The Kinect SkeletonData</param>
        /// <returns>A Processed Skeleton</returns>
        public static Skeleton ToSkeleton(this SkeletonData data)
        {
            return new Skeleton
            {
                ElbowLeft = data.To3DVector(JointID.ElbowLeft),
                ElbowRight = data.To3DVector(JointID.ElbowRight),
                WristLeft = data.To3DVector(JointID.WristLeft),
                WristRight = data.To3DVector(JointID.WristRight),
                ShoulderLeft = data.To3DVector(JointID.ShoulderLeft),
                ShoulderRight = data.To3DVector(JointID.ShoulderRight),
                PalmLeft = data.To3DVector(JointID.HandLeft),
                PalmRight = data.To3DVector(JointID.HandRight),
                ShoulderCenter = data.To3DVector(JointID.ShoulderCenter)
            };
        }

        /// <summary>
        /// Extract the concatenated feature vector from a skeleton
        /// </summary>
        /// <param name="s">A processed Skeleton</param>
        /// <returns>The concatenated feature vector</returns>
        public static double[] FeatureSet(Skeleton s)
        {
            SkeletonFeatureFrame sff = new SkeletonFeatureFrame(s);
            var features = new double[31];
            int nextFreeIndex = 0;

            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.ArmLeft, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.ArmRight, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.ForeArmLeft, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.ForeArmLeft, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.HandLeft, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.HandRight, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.ShoulderBladeLeft, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.ShoulderBladeRight, features, nextFreeIndex);
            nextFreeIndex = AddFeat(sff, SkeletonFeatureFrame.DirectionVectors.BetweenHands, features, nextFreeIndex);

            features[nextFreeIndex++] = sff.Angle(SkeletonFeatureFrame.Angles.AngleElbowLeft);
            features[nextFreeIndex++] = sff.Angle(SkeletonFeatureFrame.Angles.AngleElbowRight);
            features[nextFreeIndex++] = sff.Angle(SkeletonFeatureFrame.Angles.AngleShoulderLeft);
            features[nextFreeIndex++] = sff.Angle(SkeletonFeatureFrame.Angles.AngleShoulderRight);

            if (nextFreeIndex != features.Length) throw new Exception("Missing features");

            return features;
        }

        private static int AddFeat(
            SkeletonFeatureFrame sff, SkeletonFeatureFrame.DirectionVectors feature,
            double[] arr, int nextFreeIndex)
        {
            Vector3D v = sff.DirectionVector(feature);
            arr[nextFreeIndex++] = v.X;
            arr[nextFreeIndex++] = v.Y;
            arr[nextFreeIndex++] = v.Z;
            return nextFreeIndex;
        }
    }
}