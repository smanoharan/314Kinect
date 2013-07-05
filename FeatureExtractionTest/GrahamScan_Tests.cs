using System.Collections.Generic;
using FeatureExtraction;
using FeatureExtraction.Hand;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    /// <summary>
    /// Summary description for GrahamScan_Tests
    /// </summary>
    [TestClass]
    public class GrahamScan_Tests
    {
        [TestMethod]
        public void RemovePointTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(1, 1, 1));
            points.Add(new Point(2, 2, 2));
            points.Add(new Point(3, 3, 3));
            points.Add(new Point(4, 4, 4));
            points.Add(new Point(5, 5, 5));

            List<Point> newPoints = GrahamScan.RemovePoint(points, new Point(2, 2, 2));
            Assert.AreEqual(newPoints[0], points[0]);
            Assert.AreEqual(newPoints[1], points[2]);
            Assert.AreEqual(newPoints[2], points[3]);
            Assert.AreEqual(newPoints[3], points[4]);
        }

        [TestMethod]
        public void GetLowestPointTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(4, 4, 4));
            points.Add(new Point(5, 5, 5));
            points.Add(new Point(1, 1, 1));
            points.Add(new Point(2, 2, 2));
            points.Add(new Point(3, 3, 3));
            Assert.AreEqual(new Point(1,1,1),GrahamScan.GetLowestPoint(points));
        }

        [TestMethod]
        public void HullSquareTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(1,1,1));
            points.Add(new Point(2, 1, 1));
            points.Add(new Point(1, 2, 1));
            points.Add(new Point(2, 2, 1));
            Assert.AreEqual(4, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullSquareNegTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(-1, -1, 1));
            points.Add(new Point(1, 1, 1));
            points.Add(new Point(1, -1, 1));
            points.Add(new Point(-1, 1, 1));
            Assert.AreEqual(4, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullSquareAdvanTest()
        {
            List<Point> points = new List<Point>();
            Point start = new Point(1, 1, 1);
            points.Add(start);
            points.Add(new Point(20, 1, 1));
            points.Add(new Point(1, 20, 1));
            points.Add(new Point(20, 20, 1));
            points.Add(new Point(10, 15, 1)); //not on hull
            points.Add(new Point(15, 10, 1)); //not on hull
            Assert.AreEqual(4, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullSquareNegAdvanTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(-10, -10, 1));
            points.Add(new Point(10, 10, 1));
            points.Add(new Point(0, 0, 1)); //not on hull
            points.Add(new Point(5, 5, 1)); //not on hull
            points.Add(new Point(10, -10, 1));
            points.Add(new Point(-10, 10, 1));
            Assert.AreEqual(4, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullDiamondTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(5, 0, 1));
            points.Add(new Point(6, 1, 1));
            points.Add(new Point(7, 2, 1));
            points.Add(new Point(8, 3, 1));
            points.Add(new Point(7, 4, 1));
            points.Add(new Point(6, 5, 1));
            points.Add(new Point(5, 6, 1));
            points.Add(new Point(4, 5, 1));
            points.Add(new Point(3, 4, 1));
            points.Add(new Point(2, 3, 1));
            points.Add(new Point(3, 2, 1));
            points.Add(new Point(4, 1, 1));
            Assert.AreEqual(12, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullDiamondNegTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(0, 2, 1));
            points.Add(new Point(1, 1, 1));
            points.Add(new Point(2, 0, 1));
            points.Add(new Point(0, -2, 1));
            points.Add(new Point(1, -1, 1));
            points.Add(new Point(-1, -1, 1));
            points.Add(new Point(-2, 0, 1));
            points.Add(new Point(-1, 1, 1));
            Assert.AreEqual(8, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullDiamondAdvanTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(5, 0, 1));
            points.Add(new Point(6, 1, 1));
            points.Add(new Point(7, 2, 1));
            points.Add(new Point(8, 3, 1));
            points.Add(new Point(7, 4, 1));
            points.Add(new Point(5, 4, 1)); //not on hull
            points.Add(new Point(4, 4, 1)); //not on hull
            points.Add(new Point(6, 4, 1)); //not on hull
            points.Add(new Point(6, 5, 1));
            points.Add(new Point(5, 6, 1));
            points.Add(new Point(4, 5, 1));
            points.Add(new Point(3, 4, 1));
            points.Add(new Point(2, 3, 1));
            points.Add(new Point(3, 2, 1));
            points.Add(new Point(4, 1, 1));
            Assert.AreEqual(12, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullDiamondNegAdvanTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(0, 20, 1));
            points.Add(new Point(10, 10, 1));
            points.Add(new Point(20, 0, 1));
            points.Add(new Point(0, -20, 1));
            points.Add(new Point(0, 0, 1)); //not on hull
            points.Add(new Point(-5, -5, 1)); //not on hull
            points.Add(new Point(10, -10, 1));
            points.Add(new Point(-10, -10, 1));
            points.Add(new Point(-20, 0, 1));
            points.Add(new Point(-10, 10, 1));
            Assert.AreEqual(8, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullOctagonTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(2, 0, 1));
            points.Add(new Point(-2, 0, 1));
            points.Add(new Point(-1, 1, 1));
            points.Add(new Point(0, 1, 1));
            points.Add(new Point(0, -2, 1));
            points.Add(new Point(2, -1, 1));
            points.Add(new Point(-1, -2, 1));
            points.Add(new Point(-2, -1, 1));
            Assert.AreEqual(8, GrahamScan.Scan(points).Count);
        }

        [TestMethod]
        public void HullOctagonAdvanTest()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(200, 0, 1));
            points.Add(new Point(-200, 0, 1));
            points.Add(new Point(-100, 100, 1));
            points.Add(new Point(0, 100, 1));
            points.Add(new Point(0, -200, 1));
            points.Add(new Point(200, -100, 1));
            points.Add(new Point(0, 0, 1)); //not on hull
            points.Add(new Point(10, 10, 1)); //not on hull
            points.Add(new Point(10, -10, 1)); //not on hull
            points.Add(new Point(100, 0, 1)); //not on hull
            points.Add(new Point(-100, -200, 1));
            points.Add(new Point(-200, -100, 1));
            Assert.AreEqual(8, GrahamScan.Scan(points).Count);
        }
    }
}
