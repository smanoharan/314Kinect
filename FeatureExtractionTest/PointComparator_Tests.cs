using System.Collections.Generic;
using FeatureExtraction;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class PointComparator_Tests
    {
        [TestMethod]
        public void isLeftInclTest()
        {
            Assert.AreEqual(true, PointComparator.isLeftIncl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(1, 2, 1)));
            Assert.AreEqual(true, PointComparator.isLeftIncl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(0, 2, 1)));
            Assert.AreEqual(false, PointComparator.isLeftIncl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(0, -2, 1)));
            Assert.AreEqual(true, PointComparator.isLeftIncl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(3, 3, 1)));
        }

        [TestMethod]
        public void isLeftExclTest()
        {
            Assert.AreEqual(true, PointComparator.isLeftExcl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(1, 2, 1)));
            Assert.AreEqual(true, PointComparator.isLeftExcl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(0, 2, 1)));
            Assert.AreEqual(false, PointComparator.isLeftExcl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(0, -2, 1)));
            Assert.AreEqual(false, PointComparator.isLeftExcl(new Point(1, 1, 1), new Point(2, 2, 1), new Point(3, 3, 1)));
        }

        [TestMethod]
        public void comparatorTest()
        {
            List<Point> points = new List<Point>();
            Point start = new Point(1,1,1);
            points.Add(new Point(-1, 3, 1));
            points.Add(new Point(3,3,1));
            points.Add(new Point(2,2,1));
            points.Add(new Point(2,4,1));
            PointComparator pc = new PointComparator(start);
            points.Sort(pc.Compare);
            Assert.AreEqual(new Point(2, 2, 1), points[0]);
            Assert.AreEqual(new Point(3, 3, 1), points[1]);
            Assert.AreEqual(new Point(2, 4, 1), points[2]);
            Assert.AreEqual(new Point(-1,3,1), points[3]);
        } 
    }
}
