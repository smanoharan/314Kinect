using System;
using System.Linq;
using FeatureExtraction.Util;

namespace FeatureExtraction.Skeleton
{
    /// <summary>
    /// The set of processed features of a Skeleton.
    /// </summary>
    public class SkeletonFeatureFrame
    {
        public enum DirectionVectors
        {
            ArmLeft,
            ArmRight,
            ForeArmLeft,
            ForeArmRight,
            HandLeft,
            HandRight,
            ShoulderBladeLeft,
            ShoulderBladeRight,
            BetweenHands,
        }

        public enum Angles
        {
            AngleElbowLeft,
            AngleElbowRight,
            AngleShoulderLeft,
            AngleShoulderRight,
        }

        private readonly Vector3D[] featureVectors;
        private readonly double[] featureAngles;

        public SkeletonFeatureFrame(Skeleton skeleton)
        {
            featureVectors = FeatureVectorsOf(skeleton);
            featureAngles = FeatureAnglesOf(featureVectors);
        }

        private static Vector3D[] FeatureVectorsOf(Skeleton sk)
        {
            var result = new Vector3D[Enum.GetValues(typeof(DirectionVectors)).Length];
            result[(int)DirectionVectors.HandLeft] = DirectionVector(sk.WristLeft, sk.PalmLeft);
            result[(int)DirectionVectors.ForeArmLeft] = DirectionVector(sk.ElbowLeft, sk.WristLeft);
            result[(int)DirectionVectors.ArmLeft] = DirectionVector(sk.ShoulderLeft, sk.ElbowLeft);
            result[(int)DirectionVectors.ShoulderBladeLeft] = DirectionVector(sk.ShoulderCenter, sk.ShoulderLeft);
            
            result[(int)DirectionVectors.HandRight] = DirectionVector(sk.WristRight, sk.PalmRight);
            result[(int)DirectionVectors.ForeArmRight] = DirectionVector(sk.ElbowRight, sk.WristRight);
            result[(int)DirectionVectors.ArmRight] = DirectionVector(sk.ShoulderRight, sk.ElbowRight);
            result[(int)DirectionVectors.ShoulderBladeRight] = DirectionVector(sk.ShoulderCenter, sk.ShoulderRight);
            
            result[(int)DirectionVectors.BetweenHands] = DirectionVector(sk.PalmRight, sk.PalmLeft);
            
            return result;
        }

        private static double[] FeatureAnglesOf(Vector3D[] featureVectors)
        {
            var result = new double[Enum.GetValues(typeof(Angles)).Length];

            result[(int)Angles.AngleShoulderLeft] = Angle(
                -featureVectors[(int)DirectionVectors.ShoulderBladeLeft],
                featureVectors[(int)DirectionVectors.ArmLeft]);

            result[(int)Angles.AngleShoulderRight] = Angle(
                -featureVectors[(int)DirectionVectors.ShoulderBladeRight],
                featureVectors[(int)DirectionVectors.ArmRight]);

            result[(int)Angles.AngleElbowLeft] = Angle(
                -featureVectors[(int)DirectionVectors.ArmLeft],
                featureVectors[(int)DirectionVectors.ForeArmLeft]);

            result[(int)Angles.AngleElbowRight] = Angle(
                -featureVectors[(int)DirectionVectors.ArmRight],
                featureVectors[(int)DirectionVectors.ForeArmRight]);

            return result;
        }

        public Vector3D DirectionVector(DirectionVectors vector)
        {
            return featureVectors[(int)vector];
        }

        private static Vector3D DirectionVector(Vector3D src, Vector3D dest)
        {
            Vector3D direction = dest - src;
            direction.Normalise();
            return direction;
        }

        public double Angle(Angles angle)
        {
            return featureAngles[(int)angle];
        }

        /// <summary>
        /// NOTE: This is a special case helper method
        /// 
        /// Find the angle between two <b>Unit</b> Vectors.
        ///     i.e. this means that norm(uva) * norm(uvb) = 1
        /// 
        /// Also, uva and uvb must meet either at the heads or at the tails.
        ///     i.e. If the head of one meets the tail of another, 
        ///          Then the supplementary angle is returned!
        ///          (This is how the inverse Cosine function behaves, arccos(-x) = PI-arccos(x); )
        /// 
        /// Hence, this is a private method.
        /// </summary>
        private static double Angle(Vector3D uva, Vector3D uvb)
        {
            return Math.Acos(uva*uvb);
        }

        /// <summary>
        /// Convert and flatten all of the features into a double array.
        /// </summary>
        /// <returns>An array of doubles, representing the concatenated vector of features</returns>
        public double[] ToFeatureArray()
        {
            return featureVectors.SelectMany(v => new[] { v.X, v.Y, v.Z }).Concat(featureAngles).ToArray();
        }
    }
}