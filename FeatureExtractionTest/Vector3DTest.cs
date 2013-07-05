using System;
using FeatureExtraction;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    /// <summary>
    /// Summary description for Vector3DTes
    /// </summary>
    [TestClass]
    public class Vector3DTest
    {
        private const double Precision = 0.01;

        public void TestNormalisedExpected(Vector3D v, double expectedX, double expectedY, double expectedZ)
        {
            v.Normalise();
            TestXYZ(v, expectedX, expectedY, expectedZ);
        }

        public void TestXYZ(Vector3D v, double expectedX, double expectedY, double expectedZ)
        {
            Assert.AreEqual(expectedX, v.X, Precision);
            Assert.AreEqual(expectedY, v.Y, Precision);
            Assert.AreEqual(expectedZ, v.Z, Precision);
        }


        [TestMethod]
        public void TestScale()
        {
            Vector3D v1 = new Vector3D(1, 2, 3);
            v1 = 2 * v1;
            TestXYZ(v1, 2, 4, 6);
        }


        [TestMethod]
        public void TestAdd()
        {
            Vector3D v1 = new Vector3D(1, 2, 7);
            Vector3D v2 = new Vector3D(2, 8, 1);
            TestXYZ(v1 + v2, 3, 10, 8);
        }


        [TestMethod]
        public void TestSubtract()
        {
            Vector3D v1 = new Vector3D(1, 2, 7);
            Vector3D v2 = new Vector3D(2, 8, 1);
            TestXYZ(v1 - v2, -1, -6, 6);
        }


        [TestMethod]
        public void TestNegate()
        {
            Vector3D v1 = new Vector3D(1, 2, 7);
            TestXYZ(-v1, -1, -2, -7);
        }


        [TestMethod]
        public void TestDotProduct()
        {
            Vector3D v1 = new Vector3D(1, 2, 7);
            Vector3D v2 = new Vector3D(2, 8, 1);
            Assert.AreEqual(v1.DotProduct(v2), 25);
            Assert.AreEqual(v1*v2, 25);
        }

        [TestMethod]
        public void TestValidNormalise()
        {
            Vector3D v1 = new Vector3D(5, 4, 3);
            // Magnitude = √(5^2 + 4^2 + 3^2) = √50
            // Therefore expected: x = 5/√50, y = 4/√50, z = 3/√50
            TestNormalisedExpected(v1, 5 / Math.Sqrt(50), 4 / Math.Sqrt(50), 3 / Math.Sqrt(50));

            Vector3D v2 = new Vector3D(1, 6, 7);
            // Magnitude = √(1^2 + 6^2 + 7^2) = √86
            // Therefore expected: x = 5/√86, y = 4/√86, z = 3/√86
            TestNormalisedExpected(v2, 1 / Math.Sqrt(86), 6 / Math.Sqrt(86), 7 / Math.Sqrt(86));
        }


        [TestMethod]
        public void TestInvalidNormalise()
        {
            Vector3D zeroVector = new Vector3D(0, 0, 0);
            zeroVector.Normalise();
            TestXYZ(zeroVector, 0, 0, 0);
        }
    }
}
