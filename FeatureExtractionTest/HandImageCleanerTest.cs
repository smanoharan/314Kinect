using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FeatureExtraction;
using FeatureExtraction.Hand;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class HandImageCleanerTest
    {
        private BitArray origMask;
        private int origWidth;

        [TestInitialize]
        public void Setup()
        {
            /*
             *  origMask is a 8x8 matrix
             *  ------------------------
             *  
             *      0 0 0 1  1 0 0 0    =   0x18
             *      0 0 0 1  1 0 1 1    =   0x1b
             *      0 0 0 1  1 0 1 0    =   0x1a
             *      0 1 1 1  0 0 0 0    =   0x70   
             *      
             *      0 0 0 1  0 0 0 0    =   0x10
             *      0 0 0 0  1 0 0 0    =   0x08
             *      0 0 0 0  1 0 0 0    =   0x08
             *      0 0 0 1  1 0 0 0    =   0x18
             * 
             *      31 set bits, 33 unset bits
             */
            origWidth = 8;
            origMask = new BitArray(new byte[] { 0x18, 0x1b, 0x1a, 0x70, 0x10, 0x08, 0x08, 0x18 });
        }


        [TestMethod]
        public void TestImageGrowOfZeroDoesntChangeImage()
        {
            AssertBitArraysAreEqual(HandImageCleaner.Dilate(origMask, origWidth, 0), origMask);
        }

        [TestMethod]
        public void TestImageShrinkOfZeroDoesntChangeImage()
        {
            AssertBitArraysAreEqual(HandImageCleaner.Dilate(origMask, origWidth, 0, true), origMask);
        }

        [TestMethod]
        public void TestImageGrowOneStep()
        {
            /*
             *  after grow image of 1-step:
             *  ------------------------
             *      0 0 1 1  1 1 1 1    =   0x3f
             *      0 0 1 1  1 1 1 1    =   0x3f
             *      1 1 1 1  1 1 1 1    =   0xff
             *      1 1 1 1  1 1 1 1    =   0xff   
             *      
             *      1 1 1 1  1 1 0 0    =   0xfc
             *      0 0 1 1  1 1 0 0    =   0x3c
             *      0 0 1 1  1 1 0 0    =   0x3c
             *      0 0 1 1  1 1 0 0    =   0x3c
             * 
             */
            var expectedMask = new BitArray(new byte[] { 0x3f, 0x3f, 0xff, 0xff, 0xfc, 0x3c, 0x3c, 0x3c });
            var actual = HandImageCleaner.Dilate(origMask, origWidth, 1);
            AssertBitArraysAreEqual(actual, expectedMask);
        }

        [TestMethod]
        public void TestImageGrowTwoStep()
        {
            /*
             *  after grow image of 2-step:
             *  ------------------------
             *      0 1 1 1  1 1 1 1    =   0x7f
             *      1 1 1 1  1 1 1 1    =   0xff
             *      1 1 1 1  1 1 1 1    =   0xff
             *      1 1 1 1  1 1 1 1    =   0xff   
             *      
             *      1 1 1 1  1 1 1 1    =   0xff
             *      1 1 1 1  1 1 1 0    =   0xfe
             *      0 1 1 1  1 1 1 0    =   0x7e
             *      0 1 1 1  1 1 1 0    =   0x7e
             * 
             */
            var expectedMask = new BitArray(new byte[] { 0x7f, 0xff, 0xff, 0xff, 0xff, 0xfe, 0x7e, 0x7e });
            var actual = HandImageCleaner.Dilate(origMask, origWidth, 2);
            AssertBitArraysAreEqual(actual, expectedMask);
        }

        [TestMethod]
        public void TestImageGrowThreeStep()
        {
            // after 3 steps, all pixels are set to 1.
            var expectedMask = new BitArray(origWidth*origWidth, true);
            var actual = HandImageCleaner.Dilate(origMask, origWidth, 3);
            AssertBitArraysAreEqual(actual, expectedMask);
        }

        [TestMethod]
        public void TestImageShrinkOneStep()
        {
            // after 1 step shrink, all pixels are set to 0.
            var expectedMask = new BitArray(origWidth * origWidth, false);
            var actual = HandImageCleaner.Dilate(origMask, origWidth, 1, true);
            AssertBitArraysAreEqual(actual, expectedMask);
        }

        private static void AssertBitArraysAreEqual(BitArray actual, BitArray expected)
        {
            // check that these have the same length, and then their XOR is zero.
            Assert.AreEqual(actual.Length, expected.Length);
            var diff = actual.Xor(expected);
            for (int i = 0; i < actual.Length; i++)
                Assert.IsFalse(diff[i]);
        }

        [TestMethod]
        public void TestBlankVideoIsIgnoredForFiltering()
        {
            int videoArraySize = origWidth*origWidth*4*4;

            // a video-frame where all pixels are white
            var blankVideoWhite = new byte[videoArraySize];
            for (int i = 0; i < videoArraySize; i++) 
                blankVideoWhite[i] = 255;

            // a video-frame where all pixels are black
            var blankVideoBlack = new byte[videoArraySize];
            for (int i = 0; i < videoArraySize; i++)
                blankVideoBlack[i] = 0;

            // check that filtering with a blank video is rejected:
            Assert.IsNull(HandImageCleaner.FilterByVideo(origMask, origWidth, blankVideoWhite, origWidth));
            Assert.IsNull(HandImageCleaner.FilterByVideo(origMask, origWidth, blankVideoBlack, origWidth));
        }
        
        [TestMethod]
        public void TestImageOpenThenCloseRemovesNoise()
        {
            /*
             *  noisy image map: 
             *      there is the large blob (i.e. the hand) which needs to be retained
             *      plus the small (1x1) blobs (noise) and a hole which need to be eliminated
             *      
             *  ------------------------
             *  
             *      1 0 0 0 0 1 0 0    =   0x84
             *      0 1 0 1 0 0 0 1    =   0x51
             *      0 0 0 0 0 1 0 0    =   0x04
             *      1 0 0 0 0 0 0 0    =   0x80   
             *      0 0 1 1 1 1 0 0    =   0x3d
             *      1 1 1 1 1 1 1 1    =   0xff
             *      1 1 1 1 1 1 1 1    =   0xff
             *      1 1 1 0 1 1 1 1    =   0xef
             *      
             * 
             * expected clean map:
             * ------------------------     
             *      0 0 0 0 0 0 0 0    =   0x00
             *      0 0 0 0 0 0 0 0    =   0x00
             *      0 0 0 0 0 0 0 0    =   0x00
             *      0 0 0 0 0 0 0 0    =   0x00
             *      0 0 1 1 1 1 0 0    =   0x3c
             *      1 1 1 1 1 1 1 1    =   0xff
             *      1 1 1 1 1 1 1 1    =   0xff
             *      1 1 1 1 1 1 1 1    =   0xff
             */
            const int imageWidth = 8;
            var noisyMap = new BitArray(new byte[] { 0x84, 0x51, 0x04, 0x80, 0x3d, 0xff, 0xff, 0xef });
            var cleanMap = new BitArray(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x3c, 0xff, 0xff, 0xff });
            var actual = HandImageCleaner.RemoveNoise(noisyMap, imageWidth);
            AssertBitArraysAreEqual(actual, cleanMap);
        }
    }
}
