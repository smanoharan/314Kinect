using System;
using System.Collections.Generic;
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
using Engine;
using Engine.FeatureExtraction;
using Engine.Kinect;
using Engine.Model;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Threading;
#if KINECT
        using Model = Engine.EngineModel;
#else
        using Model = Engine.Model.MockModel;
#endif

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for NewSignRecord.xaml
	/// </summary>
	public partial class NewSignRecord : UserControl
	{
	    private Sign s; // current sign which contains the name, picture location, video location, and hand shape information
		private int currentCount; // number of training examples
		private DispatcherTimer timer; // count downs each training and to check if the user has stop movement
		private List<Sequence> examples; // the training examples or sequences
		private SkeletonDrawer skeletonDrawer; // the drawer which draws the skeleton of the trainer
		
		public NewSignRecord(Sign s)
		{
			this.InitializeComponent();
			this.Cursor = Cursors.None;
#if KINECT
			KinectHandler.Get().AddVideoListener(NUIVideoFrameReady);
			skeletonDrawer = new SkeletonDrawer(SkeletonImage);
			skeletonDrawer.StartDrawing();
            CursorController.Get().StartListening();
#endif
		    this.s = s;
			this.examples = new List<Sequence>();
			this.currentCount = -1;
            lblTrainExampleNum.Content = "Number of Training Examples: " + examples.Count;
			
		}

        public NewSignRecord(Sign s, List<Sequence> examples)
        {
            this.InitializeComponent();
			this.Cursor = Cursors.None;
#if KINECT
			KinectHandler.Get().AddVideoListener(NUIVideoFrameReady);
			skeletonDrawer = new SkeletonDrawer(SkeletonImage);
			skeletonDrawer.StartDrawing();
            CursorController.Get().StartListening();
#endif
            this.s = s;
            this.examples = examples;    
            this.currentCount = -1;
            lblTrainExampleNum.Content = "Number of Training Examples: " + examples.Count;
        }


		private void InitialiseNewTrainingExample()
		{
			// Hide hand controller
			hand.Visibility = Visibility.Hidden;
			
			// Hide all the buttons
			btnContinueDoNotKeep.Visibility = Visibility.Hidden;
			btnStopTrainingKeep.Visibility = Visibility.Hidden;
			btnStopTrainingDoNotKeep.Visibility = Visibility.Hidden;
			btnContinueKeep.Visibility = Visibility.Hidden;
			btnStartRecording.Visibility = Visibility.Hidden;
			btnBack.Visibility = Visibility.Hidden;

			// Display countdown label
			lblCountDown.Visibility = Visibility.Visible;

			currentCount = 3;
			//lblCountDown.Content = "Loading...";
			lblEntropyLow.Content = "";

			this.timer = new DispatcherTimer { IsEnabled = true, Interval = TimeSpan.FromSeconds(1) };
			timer.Tick += OnTimerEvent;
			timer.Start();
		}

		private void StopTimer()
		{
			timer.Tick -= OnTimerEvent;
			timer.Stop();
		}

		private void AddTrainingExample(Sequence recentTrainedExample)
		{
			this.examples.Add(recentTrainedExample);
            ShowOptions();
		}
		
		private void AddTrainingExampleWithVideo(Sequence recentTrainedExample, byte[][] video)
		{
            this.examples.Add(recentTrainedExample);
		    ShowOptions();
		}

        /// <summary>
        /// // Show the 4 available options upon completion of one training sequence
        /// </summary>
        /// <param name="recentTrainedExample"></param>
	    private void ShowOptions()
	    {
	        btnContinueDoNotKeep.Visibility = Visibility.Visible;
	        btnContinueKeep.Visibility = Visibility.Visible;
	        btnStopTrainingKeep.Visibility = Visibility.Visible;
	        btnStopTrainingDoNotKeep.Visibility = Visibility.Visible;
	        hand.Visibility = Visibility.Visible;
	    }


	    private void LowEntropyHandler(int remainder)
		{
			lblEntropyLow.Content = remainder;
		}

		private void HighEntropyHandler()
		{
			lblEntropyLow.Content = "";
		}

		private void OnTimerEvent(object sender, EventArgs eventArgs)
		{
			if (currentCount < 0)
			{
				lblCountDown.Content = "";
				lblCountDown.Visibility = Visibility.Hidden;
				StopTimer();
                // record this training sequence
				if(examples.Count == 0 && s.VidLocation == "") 
                    Model.RecordSequenceWithVideo(AddTrainingExampleWithVideo, LowEntropyHandler, HighEntropyHandler);
				else
                    Model.RecordSequence(AddTrainingExample, LowEntropyHandler, HighEntropyHandler);
			}
			else
			{
                // Count down each training (Count down 3...2...1.)
				lblCountDown.Content = (currentCount == 0) ? "Start!" : (currentCount).ToString();
				currentCount--;
				
			}
		}

		private void btnStartRecording_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			InitialiseNewTrainingExample();
		}


		private void btnContinueDoNotKeep_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			examples.RemoveAt(examples.Count - 1); // remove the last example
			lblTrainExampleNum.Content = "Number of Training Examples: " + examples.Count;
			InitialiseNewTrainingExample();
		}

		private void btnContinueKeep_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			lblTrainExampleNum.Content = "Number of Training Examples: " + examples.Count;
			InitialiseNewTrainingExample();
		}


		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if(examples.Count > 0)
			{
				
					if(MessageBox.Show("Training examples have been recorded. Going back means that all training examples will be discarded.\nDiscard all training examples?", "Discard all training examples?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					{
#if KINECT
						skeletonDrawer.StopDrawing();
                        CursorController.Get().StopListening();
#endif
						Switcher.Switch(new NewSignVideo(s));
					}
			}
			else
            {
#if KINECT
                skeletonDrawer.StopDrawing();
                CursorController.Get().StopListening();
#endif
                Switcher.Switch(new NewSignVideo(s));
			}
		}


		private void NUIVideoFrameReady(object sender, ImageFrameReadyEventArgs e)
		{
            // Sets the kinect video image to the canvas
			PlanarImage image = e.ImageFrame.Image;
			CameraImage.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null,
													 image.Bits, image.Width * image.BytesPerPixel);
		}

		private void btnStopTrainingDoNotKeep_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			
			examples.RemoveAt(examples.Count-1);
			
			if (examples.Count == 0)
			{
				if(MessageBox.Show("Training examples must be recorded to create the sign. Do you wish to discard the creation of the new sign?", "Discard the sign?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
#if KINECT
                    skeletonDrawer.StopDrawing();
#endif
                    Switcher.Switch(new SignManager());
				}
			}
			else
            {
#if KINECT
                skeletonDrawer.StopDrawing();
#endif
                Switcher.Switch(new NewSignFinish(s, examples));
			}
		}

		private void btnStopTrainingKeep_Click(object sender, System.Windows.RoutedEventArgs e)
		{
#if KINECT
            skeletonDrawer.StopDrawing();
#endif
			Switcher.Switch(new NewSignFinish(s, examples));
		}

		/**********************************Kinect hand controller *****************************/
		
		private void btnBack_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnBack);
		}

		private void btnStartRecording_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnStartRecording);
		}

		private void btnContinueKeep_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnContinueKeep);
		}

		private void btnContinueDoNotKeep_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnContinueDoNotKeep);
		}

		private void btnStopTrainingDoNotKeep_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnStopTrainingDoNotKeep);
		}

		private void btnStopTrainingKeep_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnStopTrainingKeep);
		}

		private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.SetPosition(e.GetPosition(this).X, e.GetPosition(this).Y);
		}
		
		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
            hand.AnimateLeave();
		}

	}
}
