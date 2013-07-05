using System.Collections;
using FeatureExtraction;
using FeatureExtraction.Hand;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class GridHandFeatureExtractorTest
    {
        private int origWidth;
        private BitArray origMask;
        private int origSetBits;
        private const double Precision = 0.0001;

        [TestInitialize]
        public void Setup()
        {
            /*
             *  origMask is a 8x8 matrix
             *  ------------------------
             *  
             *      0 0 0 1  1 0 0 0    =   0x18
             *      0 0 0 1  1 0 1 1    =   0x1b
             *      0 0 1 1  1 1 1 0    =   0x3e
             *      0 1 1 1  1 1 0 0    =   0x7c   
             *      
             *      1 1 1 1  1 1 0 0    =   0xfc
             *      0 0 1 1  1 1 0 0    =   0x3c
             *      0 0 1 1  1 0 0 0    =   0x38
             *      0 0 0 1  1 0 0 0    =   0x18
             * 
             *      31 set bits, 33 unset bits
             */
            origSetBits = 31;
            origWidth = 8; 
            origMask = new BitArray(new byte[] { 0x18, 0x1b, 0x3e, 0x7c, 0xfc, 0x3c, 0x38, 0x18 });
        }

        [TestMethod]
        public void TestRegionThresholdParameterHolds()
        {
            // when below threshold, expect 0:
            AssertFeatureArrayIsSingletonWithAThreshold(0, origSetBits + 1);

            // when at, or above threshold, expect 1:
            AssertFeatureArrayIsSingletonWithAThreshold(1, origSetBits);
            AssertFeatureArrayIsSingletonWithAThreshold(1, origSetBits - 1);
        }

        private void AssertFeatureArrayIsSingletonWithAThreshold(double expected, int regionThreshold)
        {
            var actual = GridHandFeatureExtractor.GroupByGrid(origMask, 1, origWidth, regionThreshold);

            // check this is actually a singleton
            Assert.AreEqual(1, actual.Length);

            // check the (only) array element has the expected value
            Assert.AreEqual(expected, actual[0], Precision);
        }

        [TestMethod]
        public void TestFourRegionFeatures()
        {
            // using four regions: the bit counts are 9, 6, 7, 9 
            TestFourRegionFeaturesWith(5, 1, 1, 1, 1);
            TestFourRegionFeaturesWith(6, 1, 1, 1, 1);
            TestFourRegionFeaturesWith(7, 1, 0, 1, 1);
            TestFourRegionFeaturesWith(8, 1, 0, 0, 1);
            TestFourRegionFeaturesWith(9, 1, 0, 0, 1);
            TestFourRegionFeaturesWith(10, 0, 0, 0, 0);
        }

        private void TestFourRegionFeaturesWith(int regionThreshold, double e11, double e12, double e21, double e22)
        {
            var actual = GridHandFeatureExtractor.GroupByGrid(origMask, 2, 4, regionThreshold);
            
            // check that there are exactly 4 features and they all match up to expected
            Assert.AreEqual(4, actual.Length);
            Assert.AreEqual(actual[0], e11, Precision);
            Assert.AreEqual(actual[1], e12, Precision);
            Assert.AreEqual(actual[2], e21, Precision);
            Assert.AreEqual(actual[3], e22, Precision);
        }
    }
}
