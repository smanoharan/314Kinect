using System.Collections.Generic;
using Engine.Kinect;
using FeatureExtraction;
using FeatureExtraction.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class EntropyMonitorTest
    {
        private const double Precision = 0.001;
        private const int FramesPerSecond = 2;
        private EntropyMonitor entropyMonitor;
        private Stack<List<Vector3D>> frames;

        private static List<Vector3D> BuildFrame(
            float lex, float ley, float lez, float rex, float rey, float rez,
            float lwx, float lwy, float lwz, float rwx, float rwy, float rwz)
        {
            return new List<Vector3D> 
            { 
                new Vector3D(lex, ley, lez), new Vector3D(rex, rey, rez), 
                new Vector3D(lwx, lwy, lwz), new Vector3D(rwx, rwy, rwz) 
            };
        }

        [TestInitialize]
        public void Setup()
        {
            entropyMonitor = new EntropyMonitor(FramesPerSecond);
            frames = new Stack<List<Vector3D>>();
        }

        [TestMethod]
        public void TestEntropyIsZeroWhenStandingStill()
        {
            frames.Push(BuildFrame(0,1,1,0,-1,2,3,1,0,-9,-1,0));
            frames.Push(BuildFrame(0,1,1,0,-1,2,3,1,0,-9,-1,0));
            Assert.AreEqual(0, entropyMonitor.CalculateEntropy(frames), Precision);
        }

        [TestMethod]
        public void TestEntropyWhenMovingSlightly()
        {
            frames.Push(BuildFrame(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            frames.Push(BuildFrame(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

            // expected: distSQ[(0,0,0) -> (1,1,1)] * 4 = (3*4) = 12 
            Assert.AreEqual(12, entropyMonitor.CalculateEntropy(frames), Precision);
        }

        [TestMethod]
        public void TestEntropyIsLowWhenMovementIsVerySmall()
        {
            frames.Push(BuildFrame(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            frames.Push(BuildFrame(0.01f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)); // 0.01f units in Kinect = 1 cm 
            Assert.IsTrue(entropyMonitor.IsLowEntropy(frames));
        }

        [TestMethod]
        public void TestEntropyIsHighWhenMovementIsVeryLarge()
        {
            frames.Push(BuildFrame(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            frames.Push(BuildFrame(1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)); // 1f units in Kinect = 1 m 
            Assert.IsFalse(entropyMonitor.IsLowEntropy(frames));
        }

        [TestMethod]
        public void TestEntropyWhenMovementIsOverALongTime()
        {
            // frames per second is 2, therefore only the last 2+1 frames are considered.
            frames.Push(BuildFrame(10, 10, 10, 0, 1, 2, 4, 5, 6, 22, 11, 0)); // should have no effect
            frames.Push(BuildFrame(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));      // start frame
            frames.Push(BuildFrame(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));      // intermediate frame
            frames.Push(BuildFrame(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3));      // end frame

            // total distance moved over last second is 12+(12*4)=60
            Assert.AreEqual(60, entropyMonitor.CalculateEntropy(frames), Precision);
        }
    }
}
