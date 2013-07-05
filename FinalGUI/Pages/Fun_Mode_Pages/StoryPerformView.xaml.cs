using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Engine;
using Engine.Kinect;
using Engine.Model;
using Engine.Story;
using Microsoft.Research.Kinect.Nui;
using System.IO;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for PerformView.xaml
	/// </summary>
	public partial class StoryPerformView : UserControl
	{
		private DispatcherTimer timer;
	   
		private IModel model;
		private int currentCount;
		private SkeletonDrawer skeletonDrawer;
		private Sign sign;
		private LinkedList<Scenario>.Enumerator sceneIterator;
		private readonly StringBuilder textDisplayed =  new StringBuilder();
		private String[] words;
		private int wordcount = 0;
		private DispatcherTimer wordtimer;
        
		private bool TrySetupScenario()
		{
			var current = sceneIterator.Current;
			if (current == null) return false;

			wordtimer = new DispatcherTimer();
			words = current.EnglishLine.Split(' ');
			wordtimer.IsEnabled = true;
			wordtimer.Interval = TimeSpan.FromSeconds(1);
			wordtimer.Tick += OnTimerWordReader;
			wordtimer.Start();

		    lblSignGrammar.Content = "Signs: " + current.SignLine;
		    lblPreviousLines.Text = current.PreviousLines;
																	
			sign = Sign.FromName(current.SignName);
            

			if (sign == null)
			{
				MessageBox.Show("Sign " + current.SignName + " cannot be found. Exiting");
				return false;
			}
			return true;
		}

		public StoryPerformView(LinkedList<Scenario>.Enumerator sceneIt)
		{
			InitializeComponent();
			this.Cursor = Cursors.None;
			sceneIterator = sceneIt;
			if (!TrySetupScenario())
			{
                Switcher.Switch(new FinalComplete());
				//Switcher.Switch(new StoryManagerView());
				return;
			}
			
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
            hand.Visibility = Visibility.Hidden;
			lblCountDown.Visibility = Visibility.Visible;
			lblEnglishLine.Visibility = Visibility.Hidden;
			btnBack.Visibility = Visibility.Hidden;
			btnReady.Visibility = Visibility.Hidden;
			lblSignGrammar.Visibility = Visibility.Hidden;
			lblPreviousLines.Visibility = Visibility.Hidden;
			RectangleFnModeImage.Visibility = Visibility.Hidden;
		    CameraImage.Visibility = Visibility.Visible;
		    SkeletonImage.Visibility = Visibility.Visible;
            recBackground.Visibility = Visibility.Hidden;
            lblEntropyLow.Visibility = Visibility.Visible;

			currentCount = 3;
			lblCountDown.Content = "Get Ready";
			lblEntropyLow.Content = "";

			timer.Tick += OnTimerEvent;
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
			Switcher.Switch(new StoryFeedbackView(sign, skeletonScore, handScore, sceneIterator));
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

        private void OnTimerWordReader(object sender, EventArgs eventArgs)
        {
            var current = sceneIterator.Current;
            if (words.Length <= wordcount)
            {
                wordtimer.Stop();
                return;
            }

            textDisplayed.Append(words[wordcount++]);
            textDisplayed.Append(" ");
            lblEnglishLine.Content = textDisplayed.ToString();
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(current.BgResourcePath, UriKind.Relative));

            RectangleFnModeImage.Fill = myBrush;
        }

	    private void OnTimerEvent(object sender, EventArgs eventArgs)
		{
			if (currentCount < 0)
			{
				lblCountDown.Visibility = Visibility.Hidden;
				StopTimer();
				model.EvaluateSequence(ProvideFeedback, LowEntropyHandler, HighEntropyHandler);
			}
			else
			{
				lblCountDown.Content = (currentCount == 0) ? "Start!" : (currentCount).ToString();
				currentCount--;

			}
		}
		
		private void NUIVideoFrameReady(object sender, ImageFrameReadyEventArgs e)
		{
			PlanarImage image = e.ImageFrame.Image;
			CameraImage.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null,
													 image.Bits, image.Width * image.BytesPerPixel);
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			
#if KINECT
            CursorController.Get().StopListening();
#endif	
            Switcher.Switch(new StoryManagerView());
		}

		private void btnReady_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			InitialiseSignPerform();
		}
		
		
		/************************************Kinect Controller *******************************/
		

		private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.SetPosition(e.GetPosition(this).X, e.GetPosition(this).Y);
		}
		
		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
            hand.AnimateLeave();
		}

        private void btnBack_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            hand.AnimateEnter(btnBack);
        }

        private void btnReady_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            hand.AnimateEnter(btnReady);
        }
	}
}   


