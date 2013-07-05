using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Engine.Kinect;
using Microsoft.Research.Kinect.Nui;

namespace FinalGUI
{
    public class CursorController
    {
        private static CursorController instance;
        
        public static CursorController Get()
        {
            // Singleton pattern. Ensure one CursorController
            return instance ?? (instance = new CursorController());
        }

        private double width;
        private double height;

        private bool isListening = false;

        /// <summary>
        /// Allow kinect to be utilise as a mouse
        /// </summary>
        public void StartListening()
        {
            if (!isListening)
            {
                KinectHandler.Get().SkeletonDataReady += UpdateCursorPosition;
                isListening = true;
            }
            width = Screen.PrimaryScreen.Bounds.Width;
            height = Screen.PrimaryScreen.Bounds.Height;
        }

        /// <summary>
        /// Stop the kinect from being used like a mouse
        /// </summary>
        public void StopListening()
        {
            if (isListening) KinectHandler.Get().SkeletonDataReady -= UpdateCursorPosition;
            isListening = false;
        }

        private void UpdateCursorPosition(SkeletonData skel)
        {
            var rightHandPosition = skel.Joints[JointID.HandRight].Position;

            // transform into screen co-ordinates:
            //float xcf, ycf;
            //KinectHandler.Get().GetDepthCoordsFromSkeleton(rightHandPosition, out xcf, out ycf);
            //double xC = xcf*width;
            //double yC = ycf*height;
            double xcf = rightHandPosition.X;
            double ycf = rightHandPosition.Y;

            double xC = 0.5 + Math.Min(1, Math.Max(-1, 2*(xcf)))/2;
            double yC = 0.5 + Math.Min(1, Math.Max(-1, 3*(-ycf)))/2;
            
            Cursor.Position = Lerp(Cursor.Position, xC*width, yC*height, 0.15);
        }

        private Point Lerp(Point current, double targetX, double targetY, double c)
        {
            double xd = current.X + c * (targetX - current.X);
            double yd = current.Y + c * (targetY - current.Y);
            
            return new Point((int)xd, (int)yd);
        }
    }
}
