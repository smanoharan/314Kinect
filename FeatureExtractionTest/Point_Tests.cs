using FeatureExtraction;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class Point_Tests
    {
        [TestMethod]
        public void PointEqual()
        {
            Assert.AreEqual(true,new Point(1,1,1).Equals(new Point(1,1,1)));
            Assert.AreEqual(new Point(1,1,1),new Point(1,1,1));
        }

        [TestMethod]
        public void PointNotEqual()
        {
            Assert.AreEqual(false, new Point(1, 1, 1).Equals(new Point(2, 1, 1)));
            Assert.AreNotEqual(new Point(1, 1, 1), new Point(2, 1, 1));
        }
    }
}
