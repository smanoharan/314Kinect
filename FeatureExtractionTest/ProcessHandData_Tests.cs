using System.Collections.Generic;
using FeatureExtraction;
using FeatureExtraction.Hand;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class ProcessHandData_Tests
    {
        [TestMethod]
        public void MirrorArrayTest()
        {
            int rX = 2;
            int rY = 2;
            int[,] data = new int[rX,rY];
            data[0, 0] = 1;
            data[1, 0] = 2;
            data[0, 1] = 3;
            data[1, 1] = 4;
            ProcessHandData phd = new ProcessHandData();
            data = phd.MirrorArray(data, rX, rY);
            Assert.AreEqual(1, data[1, 0]);
            Assert.AreEqual(2, data[0, 0]);
            Assert.AreEqual(3, data[1, 1]);
            Assert.AreEqual(4, data[0, 1]);
        }

        [TestMethod]
        public void SplitDataTest1()
        {
            int[,] testData = new int[4,4];
            testData[1, 1] = 100;
            testData[2, 1] = 101;
            testData[2, 2] = 102;
            ProcessHandData phd = new ProcessHandData();
            List<List<Point>> t = phd.SplitData(testData, 4, 4);
            Assert.AreEqual(3,t[0].Count);
        }

        [TestMethod]
        public void SplitDataTest2()
        {
            int[,] testData = new int[4, 4];
            testData[1, 1] = 100;
            testData[2, 1] = 101;
            testData[3, 3] = 102;
            ProcessHandData phd = new ProcessHandData();
            List<List<Point>> t = phd.SplitData(testData, 4, 4);
            Assert.AreEqual(2, t[0].Count);
            Assert.AreEqual(2, t.Count);
        }

        [TestMethod]
        public void SplitDataTest3()
        {
            int[,] testData = new int[10, 10];
            testData[1, 1] = 100;
            testData[2, 1] = 101;
            testData[3, 1] = 102;
            testData[9, 9] = 103;
            testData[8, 9] = 104;
            testData[7, 9] = 105;
            testData[6, 9] = 106;
            testData[5, 5] = 107;
            testData[5, 4]  = 108;
            ProcessHandData phd = new ProcessHandData();
            List<List<Point>> t = phd.SplitData(testData, 10, 10);
            Assert.AreEqual(3, t.Count);
        }

        [TestMethod]
        public void ListContainsPointTest()
        {
            ProcessHandData phd = new ProcessHandData();
            List<Point> points = new List<Point>();
            points.Add(new Point(1 ,1 ,1));
            points.Add(new Point(1, 2, 1));
            points.Add(new Point(2, 1, 1));
            points.Add(new Point(2, 2, 1));
            Assert.AreEqual(true,phd.ContainsPoint(points,new Point(1,2,1)));
            Assert.AreEqual(false, phd.ContainsPoint(points, new Point(3, 2, 1)));
        }
    }
}
