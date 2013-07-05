using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Engine.Kinect;
using Microsoft.Research.Kinect.Nui;

namespace FinalGUI
{
    class SkeletonDrawer
    {
        private readonly Canvas skeletonCanvas;
        
        readonly Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>
        { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0, 0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79, 84, 33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84, 33, 42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215, 86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33, 79, 84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33, 33, 84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37, 69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69, 33, 84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222, 76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))}
        };

        public SkeletonDrawer(Canvas skeletonCanvas)
        {
            this.skeletonCanvas = skeletonCanvas;
        }

        /// <summary>
        /// Begin the drawing of the skeleton.
        /// </summary>
        public void StartDrawing()
        {
            KinectHandler.Get().SkeletonDataReady += DrawSkeleton;
        }

        /// <summary>
        /// Stop the drawing of the skeleton
        /// </summary>
        public void StopDrawing()
        {
            KinectHandler.Get().SkeletonDataReady -= DrawSkeleton;
        }
        
        Polyline GetBodySegment(JointsCollection joints, Brush brush, params JointID[] ids)
        {
            PointCollection points = new PointCollection(ids.Length);
            foreach (JointID t in ids)
            {
                points.Add(GetDisplayPosition(joints[t]));
            }

            Polyline polyline = new Polyline { Points = points, Stroke = brush, StrokeThickness = 5 };
            return polyline;
        }

        private Point GetDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            KinectHandler.Get().GetDepthCoordsFromSkeleton(joint.Position, out depthX, out depthY);
            depthX = depthX * 320; //convert to 320, 240 space
            depthY = depthY * 240; //convert to 320, 240 space

            int colorX, colorY;
            KinectHandler.Get().GetVideoCoordsFromDepth(ImageResolution.Resolution640x480, (int)depthX, (int)depthY, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            return new Point((int)(skeletonCanvas.Width * colorX / 640.0), (int)(skeletonCanvas.Height * colorY / 480));
        }


        private void DrawSkeleton(SkeletonData data)
        {
            skeletonCanvas.Children.Clear();

            // Draw bones
            Brush brush = new SolidColorBrush(Colors.Green);
            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
            skeletonCanvas.Children.Add(GetBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));

            // Draw joints
            foreach (Line jointLine in
                from Joint joint in data.Joints
                let jointPos = GetDisplayPosition(joint)
                select new Line
                {
                    Stroke = jointColors[joint.ID],
                    StrokeThickness = 6,
                    X1 = jointPos.X - 3,
                    X2 = jointPos.X + 3,
                    Y1 = jointPos.Y,
                    Y2 = jointPos.Y
                }) { skeletonCanvas.Children.Add(jointLine); }
        }
    }
}