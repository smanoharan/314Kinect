using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FeatureExtraction;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class CropTest
    {
        private byte[] original;
        private int origWidth;
        private int origHeight;

        [TestInitialize]
        public void Setup()
        {
            // original is a 5x4 matrix
            origWidth = 4;
            origHeight = 5;
            original = new byte[]
            {
                1,      2,      3,      4,
                6,      7,      8,      9,
                12,     13,     14,     15,
                18,     19,     20,     21,
                24,     25,     26,     27,
            };
        }

        [TestMethod]
        public void TestCropOfWholeImageIsWholeImage()
        {
            // cropping the entire image: so no change should be seen
            byte[] actual = CropForTesting(4, 5, 0, 0);
            AssertImagesAreEqual(actual, original);
        }

        [TestMethod]
        public void TestCropOfSingleElementIsASingletonArray()
        {
            // top left:
            AssertImagesAreEqual(CropForTesting(1, 1, 0, 0), new byte[] { 1 });

            // top right:
            AssertImagesAreEqual(CropForTesting(1, 1, 3, 0), new byte[] { 4 });

            // bottom right:
            AssertImagesAreEqual(CropForTesting(1, 1, 3, 4), new byte[] { 27 });

            // bottom left:
            AssertImagesAreEqual(CropForTesting(1, 1, 0, 4), new byte[] { 24 });
        }

        [TestMethod]
        public void TestCropRowSectionIsARow()
        {
            // top row
            AssertImagesAreEqual(CropForTesting(origWidth, 1, 0, 0), new byte[] { 1, 2, 3, 4, });

            // bottom row
            AssertImagesAreEqual(CropForTesting(origWidth, 1, 0, 4), new byte[] { 24, 25, 26, 27, });

            // middle row
            AssertImagesAreEqual(CropForTesting(origWidth, 1, 0, 2), new byte[] { 12, 13, 14, 15, });

            // middle 2 row sections
            AssertImagesAreEqual(CropForTesting(origWidth, 2, 0, 1), new byte[] { 6, 7, 8, 9, 12, 13, 14, 15, });

            // last 3 rows
            AssertImagesAreEqual(CropForTesting(origWidth, 3, 0, 2), new byte[] { 12, 13, 14, 15, 18, 19, 20, 21, 24, 25, 26, 27, });
        }

        [TestMethod]
        public void TestCropColumnSectionIsARow()
        {
            // 1st col
            AssertImagesAreEqual(CropForTesting(1, origHeight, 0, 0), new byte[] { 1, 6, 12, 18, 24 });

            // last col
            AssertImagesAreEqual(CropForTesting(1, origHeight, 3, 0), new byte[] { 4, 9, 15, 21, 27, });

            // 2nd col
            AssertImagesAreEqual(CropForTesting(1, origHeight, 1, 0), new byte[] { 2, 7, 13, 19, 25, });

            // 2nd & 3rd columns
            AssertImagesAreEqual(CropForTesting(2, origHeight, 1, 0), new byte[] { 2, 3, 7, 8, 13, 14, 19, 20, 25, 26, });

            // 1-3 colums
            AssertImagesAreEqual(CropForTesting(3, origHeight, 0, 0), new byte[] { 1, 2, 3, 6, 7, 8, 12, 13, 14, 18, 19, 20, 24, 25, 26, });
        }

        [TestMethod]
        public void TestRectangularCropWorks()
        {
            // a 2x2 rectangle (square) at 1,1 -> 2,2
            AssertImagesAreEqual(CropForTesting(2, 2, 1, 1), new byte[] { 7, 8, 13, 14, });

            // a 3x2 rectangle : 1,1->2,3
            AssertImagesAreEqual(CropForTesting(2, 3, 1, 1), new byte[] { 7, 8, 13, 14, 19, 20, });
        }

        [TestMethod]
        public void TestHandCroppingIsCenteredAtTheHand()
        {
            // crop a 2x2 square at position 2,1. 
            AssertImagesAreEqual(CropAtHandForTesting(2, 1, 1), new byte[] { 2, 3, 7, 8 });
        }

        [TestMethod]
        public void TestHandCroppingIsLowerBoundedToOriginalImage()
        {
            // crop a 2x2 square at position 0,0. This is expected to be re-centered to 1,1 (so startX=startY=0)
            AssertImagesAreEqual(CropAtHandForTesting(0, 0, 1, 0, 0), new byte[] {1, 2, 6, 7});
        }

        [TestMethod]
        public void TestHandCroppingIsUpperBoundedToOriginalImage()
        {
            // crop a 2x2 square at position 3,4. This is expected to be re-centered to 2,3 (so startX=2, startY=3)
            AssertImagesAreEqual(CropAtHandForTesting(3, 4, 1, 2, 3), new byte[] { 20, 21, 26, 27 });
        }

        private byte[] CropAtHandForTesting(int handX, int handY, int cropRadius)
        {
            return CropUtil.CropImageToHandPosition(handX, handY, cropRadius, original, origHeight, origWidth, 1);
        }

        private byte[] CropAtHandForTesting(int handX, int handY, int cropRadius, int expectedStartX, int expectedStartY)
        {
            int startX;
            int startY;
            var result = CropUtil.CropImageToHandPosition(handX, handY, cropRadius, original, 
                origHeight, origWidth, 1, out startX, out startY);

            Assert.AreEqual(startX, expectedStartX);
            Assert.AreEqual(startY, expectedStartY);
            return result;
        }

        private byte[] CropForTesting(int croppedWidth, int croppedHeight, int startX, int startY)
        {
            return CropUtil.Crop(
                original, origWidth, 1,
                croppedWidth, croppedHeight, startX, startY);
        }

        private static void AssertImagesAreEqual(byte[] actual, byte[] expected)
        {
            const string errMsg = "Unequal lengths: Actual={0}, Expected={1}";
            Assert.AreEqual(actual.Length, expected.Length, errMsg, actual.Length, expected.Length);
            Assert.IsFalse(actual.Zip(expected, (a, b) => a != b).Any(a => a));
        }
    }
}
