using System;
using Engine.FeatureExtraction;
using FeatureExtraction;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DirVec = FeatureExtraction.Skeleton.SkeletonFeatureFrame.DirectionVectors;
using Angles = FeatureExtraction.Skeleton.SkeletonFeatureFrame.Angles;
namespace EngineTest
{
    [TestClass]
    public class SkeletonFeatureExtractorTest
    {
        private const double Precision = 0.001;
        private Skeleton skeleton;
        private SkeletonFeatureFrame skeletonFeatureFrame;

        [TestInitialize]
        public void Setup()
        {
            skeleton = new Skeleton
            {
                ShoulderCenter = Vector3D.Zero,
                ShoulderLeft = Vector3D.UnitX,
                ShoulderRight = -Vector3D.UnitX
            };

            skeleton.ElbowLeft = skeleton.ShoulderLeft + 2 * Vector3D.UnitX;
            skeleton.WristLeft = skeleton.ElbowLeft + (Vector3D.UnitY);
            skeleton.PalmLeft = skeleton.WristLeft + (0.5 * Vector3D.UnitZ);

            skeleton.ElbowRight = skeleton.ShoulderRight + new Vector3D(-1.5, 0, -1.5);
            skeleton.WristRight = skeleton.ElbowRight + new Vector3D(0, 3.5, -3.5);
            skeleton.PalmRight = skeleton.WristRight + new Vector3D(-0.5, 1, -1);

            skeletonFeatureFrame = new SkeletonFeatureFrame(skeleton);
        }

        [TestMethod]
        public void TestDirectionVectors()
        {
            TestDirectionIs(Vector3D.UnitZ, DirVec.HandLeft);
            TestDirectionIs(Vector3D.UnitY, DirVec.ForeArmLeft);
            TestDirectionIs(Vector3D.UnitX, DirVec.ArmLeft);
            TestDirectionIs(Vector3D.UnitX, DirVec.ShoulderBladeLeft);

            double sqHalf = Math.Sqrt(0.5);
            const double third = 1.0 / 3;
            TestDirectionIs(new Vector3D(-third, 2 * third, -2 * third), DirVec.HandRight);
            TestDirectionIs(new Vector3D(0, sqHalf, -sqHalf), DirVec.ForeArmRight);
            TestDirectionIs(new Vector3D(-sqHalf, 0, -sqHalf), DirVec.ArmRight);
            TestDirectionIs(-Vector3D.UnitX, DirVec.ShoulderBladeRight);

            TestDirectionIs(Math.Sqrt(2 / 181.0) * new Vector3D(6, -3.5, 6.5), DirVec.BetweenHands);
        }

        private void TestDirectionIs(Vector3D expected, DirVec directionVector)
        {
            Vector3D actual = skeletonFeatureFrame.DirectionVector(directionVector);
            string message = String.Format("Expected {0} but actual was {1}", expected, actual);
            Assert.AreEqual(expected.X, actual.X, Precision, message);
            Assert.AreEqual(expected.Y, actual.Y, Precision, message);
            Assert.AreEqual(expected.Z, actual.Z, Precision, message);
        }

        [TestMethod]
        public void TestAngles()
        {
            TestAngleIs(0.75 * Math.PI, Angles.AngleShoulderRight);
            TestAngleIs(Math.PI, Angles.AngleShoulderLeft);
            TestAngleIs(2 * Math.PI / 3.0, Angles.AngleElbowRight);
            TestAngleIs(Math.PI / 2.0, Angles.AngleElbowLeft);
        }

        private void TestAngleIs(double expectedAngle, Angles directionVector)
        {
            Assert.AreEqual(expectedAngle, skeletonFeatureFrame.Angle(directionVector), Precision);
        }

        [TestMethod]
        public void TestToFeatureArrayConversion()
        {
            const double third = 1.0 / 3; 
            double sqHalf = Math.Sqrt(0.5);
            double sLe = Math.Sqrt(2/181.0); // square root of length of hand-to-hand distance.


            var actual = skeletonFeatureFrame.ToFeatureArray();
            var expected = new []
            {
                1,0,0,                  // ARM Left
                -sqHalf,0,-sqHalf,      // ARM Right
                0,1,0,                  // FOREARM Left
                0,sqHalf,-sqHalf,       // FOREARM Right
                0,0,1,                  // HAND Left
                -third,2*third,-2*third,// HAND Right
                1,0,0,                  // SHOULDER BLADE Left
                -1,0,0,                 // SHOULDER BLADE Right
                6*sLe,-3.5*sLe,6.5*sLe, // BETWEEN Hands
                Math.PI / 2.0,          // Left ELBOW Angle 
                2 * Math.PI / 3.0,      // Right ELBOW Angle 
                Math.PI,                // Left SHOULDER Angle
                0.75 * Math.PI          // Right SHOULDER Angle
            };


            for (int i = 0; i < expected.Length; i++)
            {
                string msg = String.Format("Expected {0} but was {1} (i={2})", expected[i], actual[i], i);
                Assert.AreEqual(expected[i], actual[i], Precision, msg);
            }
        }
    }
}
