using System;
using System.Collections.Generic;
using System.Linq;
using Engine.FeatureExtraction;
using FeatureExtraction;
using FeatureExtraction.Skeleton;
using FeatureExtraction.Util;
using Microsoft.Research.Kinect.Nui;

namespace Engine.Kinect
{
    /// <summary>
    /// An abstraction for dealing with the Kinect.
    /// </summary>
    public class KinectHandler
    {
        public const int FramesPerSecond = 30;
        protected static KinectHandler Instance;
        public static KinectHandler Get()
        {
            return Instance ?? 
                (Instance = new KinectHandler(true));
        }

        private readonly Runtime kinectNUI;

        /// <summary>
        /// Listen for when a tracked skeleton (raw) data becomes available
        /// </summary>
        public event Action<SkeletonData> SkeletonDataReady;

        /// <summary>
        /// Listen for when a tracked skeleton (processed) becomes available
        /// </summary>
        public event Action<Skeleton> SkeletonReady;

        protected KinectHandler(bool init)
        {
            SkeletonDataReady += s => { };
            SkeletonReady += s => { };

            if (!init) return;
            kinectNUI = new Runtime();
            kinectNUI.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex);
            kinectNUI.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            kinectNUI.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            kinectNUI.SkeletonFrameReady += DataReady;
        }

        private void DataReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var firstSkeletonData = e.SkeletonFrame.Skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
            if (firstSkeletonData == null) return;
            SkeletonDataReady(firstSkeletonData);
            InvokeSkeletonReady(firstSkeletonData.ToSkeleton());
        }

        protected void InvokeSkeletonReady(Skeleton skel)
        {
            SkeletonReady(skel);
        }

        /// <summary>
        /// Convert Co-ordinates in Depth-image space to Video-frame space.
        /// </summary>
        /// <param name="colorResolution">The resoultion of the video</param>
        /// <param name="depthX">The depth x-ordinate (in [0,1] space)</param>
        /// <param name="depthY">The depth y-ordinate (in [0,1] space)</param>
        /// <param name="videoX">(output) x-ordinate in video-frame space</param>
        /// <param name="videoY">(output) y-ordinate in video-frame space</param>
        public void GetVideoCoordsFromDepth(ImageResolution colorResolution, 
            int depthX, int depthY, out int videoX, out int videoY)
        {
            // defer the calculation to the Kinect driver
            kinectNUI.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(
                colorResolution, new ImageViewArea(), depthX, depthY, 0, out videoX, out videoY);
        }

        /// <summary>
        /// Convert skeleton co-ordinate to depth-space co-ordinates
        /// </summary>
        /// <param name="handPos">The position of the hand as per the skeleton</param>
        /// <param name="depthXf">(output) x-ordinate in [0,1] space</param>
        /// <param name="depthYf">(output) y-ordinate in [0,1] space</param>
        public void GetDepthCoordsFromSkeleton(Vector3D handPos, out float depthXf, out float depthYf)
        {
            // defer to the Kinect driver (needs conversion to Kinect Vector format)
            var kinectVector = new Vector() {W = 0, X = (float) handPos.X, Y = (float) handPos.Y, Z = (float) handPos.Z};
            GetDepthCoordsFromSkeleton(kinectVector, out depthXf, out depthYf);
        }

        /// <summary>
        /// Convert skeleton co-ordinate to depth-space co-ordinates
        /// </summary>
        /// <param name="handPos">The position of the hand as per the skeleton</param>
        /// <param name="depthXf">(output) x-ordinate in [0,1] space</param>
        /// <param name="depthYf">(output) y-ordinate in [0,1] space</param>
        public void GetDepthCoordsFromSkeleton(Vector handPos, out float depthXf, out float depthYf)
        {
            // defer to the Kinect driver
            kinectNUI.SkeletonEngine.SkeletonToDepthImage(handPos, out depthXf, out depthYf);
        }

        public virtual void AddVideoListener(EventHandler<ImageFrameReadyEventArgs> videoFrameReady)
        {
            kinectNUI.VideoFrameReady += videoFrameReady;
        }

        public virtual void AddDepthListener(EventHandler<ImageFrameReadyEventArgs> depthFrameReady)
        {
            kinectNUI.DepthFrameReady += depthFrameReady;
        }

        public virtual void RemoveDepthListener(EventHandler<ImageFrameReadyEventArgs> depthFrameReady)
        {
            kinectNUI.DepthFrameReady -= depthFrameReady;
        }

        public virtual void RemoveVideoListener(EventHandler<ImageFrameReadyEventArgs> videoFrameReady)
        {
            kinectNUI.VideoFrameReady -= videoFrameReady;
        }

        /// <summary>
        /// Stop and Unload the Kinect Driver.
        /// </summary>
        public static void Unload()
        {
            if (Instance != null) Instance.kinectNUI.Uninitialize();
            Instance = null;
        }
    }
}
