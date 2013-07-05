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
using System.IO;
using System.Windows.Threading;
using Engine.FeatureExtraction;
using Engine.Model;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for NewSignFinish.xaml
	/// </summary>
	public partial class NewSignFinish : UserControl
	{
		private string name; // name of the new sign
		private string pictureURL; // picture location of the new sign
		private HandShapes signClass; // hand shape (class) of the new sign
		private string videoURL; // video location of the new sign
		private List<Sequence>examples; // training sequences of the new sign
	    private Sign s; // the 'default' sign containing the relevant sign information without training
        private DispatcherTimer timer; // timer to update process label
        private int processingCounter; // counter used to calcualte the '.'s in the progress
		private AsyncDelegateCommand trainingCommand; // asynchronously train the sign (without affecting the processing label)
		private bool isTrainingDone = false;
		
		public NewSignFinish(Sign s, List<Sequence> examples)
		{
			this.InitializeComponent();
			this.Cursor = Cursors.None;
			this.name = s.SignName;
			this.pictureURL = s.ImgLocation;
			this.signClass = s.SignClass;
			this.videoURL = s.VidLocation;
			this.examples = examples;
		    this.s = s;
			processingCounter = 0;
		}


		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new NewSignRecord(s, examples));
		}

		private void btnProceed_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            // hide the controls once the training of the sign begins.
			lblTitle.Visibility = Visibility.Hidden;
			btnBack.Visibility = Visibility.Hidden;
			btnProceed.Visibility = Visibility.Hidden;
			lblProcessing.Visibility = Visibility.Visible;
			
			this.timer = new DispatcherTimer { IsEnabled = true, Interval = TimeSpan.FromSeconds(1) };
			timer.Tick += OnTimerEvent;
			timer.Start();

			trainingCommand = new AsyncDelegateCommand(action: CreateSignModel);
			trainingCommand.Execute(null);
		}
		
		public void CreateSignModel()
		{
#if KINECT
			Sign newSign = new Sign(name, signClass, pictureURL, videoURL, EngineModel.CreateFromTrainingSequences(examples));
#else
            Sign newSign = new Sign(name, signClass, pictureURL, videoURL, null);
#endif
            Sign.ToFile(newSign);
			isTrainingDone = true;
		}
		
		private void OnTimerEvent(object sender, EventArgs eventArgs)
		{
			if (isTrainingDone)
			{
				timer.Stop();
				timer.Tick -= OnTimerEvent;
				Switcher.Switch(new NewSignComplete());
				return;
			}

			processingCounter++;
			string processingDots = "";
			for(int i = 0; i < processingCounter % 4; i++)
			{
				processingDots += ".";
			}
			
			lblProcessing.Content = "Processing" + processingDots;
		}

		/***********************************Kinect Hand controller **************************/
		
		
		private void btnProceed_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnProceed);
		}

		private void btnBack_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnBack);
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