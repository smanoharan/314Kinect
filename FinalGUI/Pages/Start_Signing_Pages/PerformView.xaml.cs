using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Engine;
using Engine.Kinect;
using Engine.Model;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Media.Animation;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for PerformView.xaml
	/// </summary>
	public partial class PerformView : UserControl
	{
		private DispatcherTimer timer; // timer to indicate the change in movement
		private IModel model; // engine model for the recognition of the sign
		private int currentCount; // stores the countdown 
		private SkeletonDrawer skeletonDrawer;
		private Sign sign; // sign being performed

		public PerformView(Sign sign)
		{
			this.InitializeComponent();
			this.Cursor = Cursors.None;
            this.sign = sign;
#if KINECT
			skeletonDrawer = new SkeletonDrawer(SkeletonImage);
			skeletonDrawer.StartDrawing();
            model = sign.SignModel;
			KinectHandler.Get().AddVideoListener(NUIVideoFrameReady);
            CursorController.Get().StartListening();
#else
			model = new MockModel();
#endif
			this.currentCount = -1;
			this.timer = new DispatcherTimer { IsEnabled = true, Interval = TimeSpan.FromSeconds(1) };
		}

		private void InitialiseSignPerform()
		{
			// Hide hand controller
			hand.Visibility = Visibility.Hidden;
			
            lblCountDown.Visibility = Visibility.Visible;
			btnBack.Visibility = Visibility.Hidden;
			btnReady.Visibility = Visibility.Hidden;

			currentCount = 3; // set countdown
			lblCountDown.Content = "Get Ready";
			lblEntropyLow.Content = "";

			timer.Tick += OnTimerEvent; // begin countdown
			timer.Start();
		}

		private void ProvideFeedback(int skeletonScore, int handScore)
		{
			/*
#if KINECT
			KinectHandler.Unload();

			skeletonDrawer.StopDrawing();
#endif	
			*/
            Switcher.Switch(new FeedbackView(sign, skeletonScore, handScore));
		}


		private void LowEntropyHandler(int remainder)
		{
			lblEntropyLow.Content = remainder;
		}

		private void HighEntropyHandler()
		{
			lblEntropyLow.Content = "";
		}

		private void StopTimer()
		{
			timer.Tick -= OnTimerEvent;
			timer.Stop();
		}

		private void OnTimerEvent(object sender, EventArgs eventArgs)
		{
            // check if countdown is done. i.e. it is recording
			if (currentCount < 0)
			{
				lblCountDown.Visibility = Visibility.Hidden;
				StopTimer();
                // evaluate the performance of the user
				model.EvaluateSequence(ProvideFeedback, LowEntropyHandler, HighEntropyHandler);
			}
			else
			{
                   // counts down 3...2...1...start
				lblCountDown.Content = (currentCount == 0) ? "Start!" : (currentCount).ToString();
				currentCount--;
			}
		}


		private void NUIVideoFrameReady(object sender, ImageFrameReadyEventArgs e)
		{
            // loads the kinect video image to the canvas
			PlanarImage image = e.ImageFrame.Image;
			CameraImage.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null,
													 image.Bits, image.Width * image.BytesPerPixel);
		}

		
		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
#if KINECT
            CursorController.Get().StopListening();
#endif
            Switcher.Switch(new PreviewView(sign));
		}
		


		private void btnReady_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			InitialiseSignPerform();
		}



		/***********************************Kinect Hand controller**********************************************/

		
		private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
            hand.SetPosition(e.GetPosition(this).X, e.GetPosition(this).Y);

		}
		
		private void btnReady_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnReady);
		}
		
		private void btnBack_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnBack);
		}

		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
            hand.AnimateLeave();
		}




		

	
	}
}