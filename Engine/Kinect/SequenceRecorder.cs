using System;
using System.Collections.Generic;
using System.Linq;
using Engine.FeatureExtraction;
using FeatureExtraction;
using FeatureExtraction.Hand;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;
using Microsoft.Research.Kinect.Nui;

namespace Engine.Kinect
{
    /// <summary>
    /// Records a sequence of frames by capturing depth, video and skeleton output from Kinect.
    /// </summary>
    public class SequenceRecorder
    {
        // The methods used for preprocessing and feature extraction
        protected static IPreprocessor Preprocessor = new DepthMapPreprocessor();
        protected static IHandFeatureExtractor HandFeatureExtractor = new GridHandFeatureExtractor();

        protected static double EntropyInterval = 0.5;
        public const int LowEntropyCountNeeded = 3;

        // using the Singleton pattern:
        protected static SequenceRecorder Instance;
        public static SequenceRecorder Get()
        {
            return Instance ?? (Instance = new SequenceRecorder(
                new EntropyMonitor(KinectHandler.FramesPerSecond)));
        }

        private readonly EntropyMonitor entropyMonitor;
        private readonly Stack<List<Vector3D>> frameHistory;
        private readonly List<PreprocessedFrame> frames;

        private Action<Sequence> onRecordingFinishedEventHandler;
        private Action<Sequence, byte[][]> onVideoRecordingFinishedEventHandler;
        private Action<int> onLowEntropyEventHandler;
        private Action onHighEntropyEventHandler;

        private int lowEntropyFrameCount;
        private DateTime lastTime = DateTime.Now;
        protected byte[] lastVideo;
        protected byte[] lastDepth;
        private IList<byte[]> videoSequence;
        private bool recordVideo;

        protected SequenceRecorder(EntropyMonitor entropyMonitor)
        {
            this.entropyMonitor = entropyMonitor;
            frameHistory = new Stack<List<Vector3D>>();
            frames = new List<PreprocessedFrame>();
        }
        
        /// <summary>
        /// Record a sequence, wait until silence is observed (user stands still) then 
        ///   output the extracted features to OnFinishedHandler.
        /// 
        /// Also, record video and return it as a parameter of onFinishedHandler
        /// </summary>
        /// <param name="onFinishedHandler">The function to call with the output as a parameter</param>
        /// <param name="onLowEntropyHandler">The function to call when partial silence is observed</param>
        /// <param name="onHighEntropyHandler">The function to call when the user is moving</param>
        public void RecordSequence(Action<Sequence, byte[][]> onFinishedHandler,
            Action<int> onLowEntropyHandler, Action onHighEntropyHandler)
        {
            videoSequence = new List<byte[]>();
            onVideoRecordingFinishedEventHandler = onFinishedHandler;
            RecordSequence(OnFinishedRecordingVideo, onLowEntropyHandler, onHighEntropyHandler);
            recordVideo = true;
        }

        private void OnFinishedRecordingVideo(Sequence sequence)
        {
            onVideoRecordingFinishedEventHandler(sequence, videoSequence.ToArray());
            videoSequence.Clear();
        }

        /// <summary>
        /// Record a sequence, wait until silence is observed (user stands still) then 
        ///   output the extracted features to OnFinishedHandler
        /// </summary>
        /// <param name="onFinishedHandler">The function to call with the output as a parameter</param>
        /// <param name="onLowEntropyHandler">The function to call when partial silence is observed</param>
        /// <param name="onHighEntropyHandler">The function to call when the user is moving</param>
        public void RecordSequence(Action<Sequence> onFinishedHandler,
            Action<int> onLowEntropyHandler, Action onHighEntropyHandler)
        {
            // Start Recording
            frameHistory.Clear();
            frames.Clear();
            lowEntropyFrameCount = 0;

            onRecordingFinishedEventHandler = onFinishedHandler;
            onLowEntropyEventHandler = onLowEntropyHandler;
            onHighEntropyEventHandler = onHighEntropyHandler;

            KinectHandler.Get().SkeletonReady += SkeletonFrameReady;
            KinectHandler.Get().AddDepthListener(DepthReady);
            KinectHandler.Get().AddVideoListener(VideoReady);

            recordVideo = false;
        }

        private void DepthReady(object sender, ImageFrameReadyEventArgs e)
        {
            lastDepth = e.ImageFrame.Image.Bits;
        }

        private void VideoReady(object sender, ImageFrameReadyEventArgs e)
        {
            lastVideo = e.ImageFrame.Image.Bits;

            if (recordVideo) 
                videoSequence.Add(e.ImageFrame.Image.Bits);

            // TODO: Watch out for files which are too big to fit in memory
        }

        /// <summary>
        /// Stop recording the sequence
        /// </summary>
        public void CancelRecording()
        {
            KinectHandler.Get().SkeletonReady -= SkeletonFrameReady;
            KinectHandler.Get().RemoveDepthListener(DepthReady);
            KinectHandler.Get().RemoveVideoListener(VideoReady);
        }

        private static double[] ExtractHandFeatures(PreprocessedFrame frame)
        {
            return HandFeatureExtractor.ExtractFeatures(frame.CroppedVideoLeft, frame.DepthMapLeft)
                .Concat(HandFeatureExtractor.ExtractFeatures(frame.CroppedVideoRight, frame.DepthMapRight))
                .ToArray();
        }

        private void RecordingFinished()
        {
            CancelRecording();
            onRecordingFinishedEventHandler(
                new Sequence(
                    frames.Select(f => f.SkeletonFeatures).ToArray(),
                    frames.Select(ExtractHandFeatures).ToArray()));
        }

        private void ProcessEntropy()
        {
            if (DateTime.Now.Subtract(lastTime) < TimeSpan.FromSeconds(EntropyInterval)) return;

            if (entropyMonitor.IsLowEntropy(frameHistory)) 
            {
                int remaining = LowEntropyCountNeeded - lowEntropyFrameCount++;
                if (remaining <= 0) RecordingFinished();
                else onLowEntropyEventHandler(remaining);
            }
            else
            {
                lowEntropyFrameCount = 0;
                onHighEntropyEventHandler();
            }

            lastTime = DateTime.Now;
        }

        private void SkeletonFrameReady(Skeleton skel)
        {
            if (skel == null || lastVideo == null || lastDepth == null) return;

            frameHistory.Push(EntropyMonitor.ToVectorList(skel));
            frames.Add(Preprocessor.Preprocess(skel, lastVideo, lastDepth));
            ProcessEntropy();
        }
    }
}
