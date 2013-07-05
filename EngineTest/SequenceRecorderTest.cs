using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.FeatureExtraction;
using Engine.Kinect;
using FeatureExtraction;
using FeatureExtraction.Hand;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;
using Microsoft.Research.Kinect.Nui;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class SequenceRecorderTest
    {
        private const int FramesPerSecond = 2;

        class MockPreprocessor : DepthMapPreprocessor
        {
            public override PreprocessedFrame Preprocess(Skeleton skel, byte[] video, byte[] depth)
            {
                //var features = SkeletonFeatureFrame.SkeletonDataToFeatureVector(skel);
                var features = SkeletonDataAdapter.FeatureSet(skel);

                return new PreprocessedFrame(features, null, null, null, null);
            }
        }

        class MockHandProcessor : GridHandFeatureExtractor
        {
            public override double[] ExtractFeatures(byte[] video, System.Collections.BitArray depth)
            {
                return new double[] {1};
            }
        }


        class MockSeqRecorder : SequenceRecorder
        {
            static MockSeqRecorder()
            {
                Instance = new MockSeqRecorder(new EntropyMonitor(FramesPerSecond));
                EntropyInterval = 0;
                Preprocessor = new MockPreprocessor();
                HandFeatureExtractor = new MockHandProcessor();
            }

            public MockSeqRecorder(EntropyMonitor entropyMonitor) : base(entropyMonitor)
            {
                lastDepth = new byte[2*320*240];
                lastVideo = new byte[4*640*480];
            }
        }

        class MockKinectHandler : KinectHandler
        {
            static MockKinectHandler()
            {
                Instance = new MockKinectHandler();
            }

            public MockKinectHandler(): base(false)
            {
            }

            public void Invoke(Skeleton skel)
            {
                InvokeSkeletonReady(skel);
            }

            public override void AddDepthListener(EventHandler<ImageFrameReadyEventArgs> depthFrameReady)
            {
                
            }

            public override void RemoveDepthListener(EventHandler<ImageFrameReadyEventArgs> depthFrameReady)
            {
                
            }

            public override void AddVideoListener(EventHandler<ImageFrameReadyEventArgs> videoFrameReady)
            {
                
            }

            public override void RemoveVideoListener(EventHandler<ImageFrameReadyEventArgs> videoFrameReady)
            {
                
            }
        }

        private Skeleton prototypeSkeleton = new Skeleton 
        {
            ShoulderCenter = new Vector3D(0, 0, 0),
            ShoulderLeft = new Vector3D(1, 0, 0),
            ShoulderRight = new Vector3D(-1, 0, 0),
            ElbowLeft = new Vector3D(1, 1, 0),
            ElbowRight = new Vector3D(-1, 1, 0),
            WristLeft = new Vector3D(1, 1, 1),
            WristRight = new Vector3D(-1, 1, -1),
            PalmLeft = new Vector3D(1, 2, 2),
            PalmRight = new Vector3D(1, 2, 0)
        };

        private const double Precision = 0.001;

        private static Vector3D Clone(Vector3D cur)
        {
            return new Vector3D(cur.X, cur.Y, cur.Z);
        }
        private static Skeleton Clone(Skeleton cur)
        {
            return new Skeleton
            {
                ShoulderCenter = Clone(cur.ShoulderCenter),
                ShoulderLeft = Clone(cur.ShoulderLeft),
                ShoulderRight = Clone(cur.ShoulderRight),
                ElbowLeft = Clone(cur.ElbowLeft),
                ElbowRight = Clone(cur.ElbowRight),
                WristLeft = Clone(cur.WristLeft),
                WristRight = Clone(cur.WristRight),
                PalmLeft = Clone(cur.PalmLeft),
                PalmRight = Clone(cur.PalmRight)
            };
        }

        [TestInitialize]
        public void Setup()
        {
            new MockKinectHandler();
            new MockSeqRecorder(null);
        }

        [TestMethod]
        public void TestShortSequenceIsRecorded()
        {
            // just provide a short sequence of low entropies
            bool eventRaised = false;
            int lowEntropyFrames = 0;
            List<double[]> expectedSeq = new List<double[]>();

            SequenceRecorder.Get().RecordSequence(
                s => { eventRaised = true; AssertSequencesAreEqual(expectedSeq.ToArray(), s.SkeletonFeatures); },
                i => { lowEntropyFrames = lowEntropyFrames + 1; }, 
                () => Assert.Fail("Unexpected high entropy"));

            for (int i = 0; i <= SequenceRecorder.LowEntropyCountNeeded; i++)
            {
                expectedSeq.Add(SkeletonDataAdapter.FeatureSet(prototypeSkeleton));
                ((MockKinectHandler)KinectHandler.Get()).Invoke(prototypeSkeleton);
            }

            Assert.IsTrue(eventRaised);
            Assert.AreEqual(lowEntropyFrames, SequenceRecorder.LowEntropyCountNeeded);
        }

        [TestMethod]
        public void TestHighEntropyIsRecorded()
        {
            // provide a sequence of high entropies followed by silence
            bool eventRaised = false;
            int actualHighEntropyFrames = 0;
            const int expectedHighEntropyFrames = 10;
            List<double[]> expectedSeq = new List<double[]>();

            SequenceRecorder.Get().RecordSequence(
                s => { eventRaised = true; AssertSequencesAreEqual(expectedSeq.ToArray(), s.SkeletonFeatures); },
                i => { },
                () => { actualHighEntropyFrames = actualHighEntropyFrames + 1; });

            for (int i = 0; i < expectedHighEntropyFrames; i++)
            {
                prototypeSkeleton = Clone(prototypeSkeleton);
                prototypeSkeleton.WristLeft.X += 100;
                prototypeSkeleton.WristLeft.Y += 100;
                prototypeSkeleton.WristLeft.Z += 100;

                expectedSeq.Add(SkeletonDataAdapter.FeatureSet(prototypeSkeleton));
                ((MockKinectHandler)KinectHandler.Get()).Invoke(prototypeSkeleton);
            }

            for (int i = 0; i <= SequenceRecorder.LowEntropyCountNeeded + 3; i++)
            {
                expectedSeq.Add(SkeletonDataAdapter.FeatureSet(prototypeSkeleton));
                ((MockKinectHandler)KinectHandler.Get()).Invoke(prototypeSkeleton);
            }

            Assert.IsTrue(eventRaised);
            Assert.AreEqual(expectedHighEntropyFrames, actualHighEntropyFrames);
        }

        private static void AssertSequencesAreEqual(double[][] expected, double[][] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            Assert.IsTrue(expected.Zip(actual, AssertArraysAreEqual).All(a=>a));
        }

        private static bool AssertArraysAreEqual(IEnumerable<double> expected, IEnumerable<double> actual)
        {
            return expected.Zip(actual, (a,b)=>Math.Abs(a-b)).All(e=>e<Precision);
        }
    }
}
