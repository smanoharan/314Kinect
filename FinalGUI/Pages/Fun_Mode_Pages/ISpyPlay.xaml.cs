using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Engine;
using Engine.Kinect;
using Engine.Model;
using Microsoft.Research.Kinect.Nui;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for ISpyPlay.xaml
	/// </summary>
	public partial class ISpyPlay : UserControl
	{
		private List<ISpyItem> iSpyGames;
		private IModel currentSignModel;
		private DispatcherTimer timer;
	    private int CurNI = 0;
	    private int CurNW = 0;
		private int currentTimerVal = -1;
	    private SkeletonDrawer skeletonDrawer;

	    public ISpyPlay()
		{
			this.InitializeComponent();
			this.Cursor = Cursors.None;
            Initialise(); 
			randomGame(-1,-1);
			
		}

        public ISpyPlay(int ni, int nw, bool retry)
        {
            this.InitializeComponent();
			this.Cursor = Cursors.None;
            Initialise();
            if(retry)   SetGame(ni, nw);
            else        randomGame(ni,nw);
        }

        private void Initialise()
        {
            iSpyGames = getFiles(Directory.GetCurrentDirectory() + "\\ISpy"); //Get the ispy game files
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1), IsEnabled = false };
        }

		private List<ISpyItem> getFiles(string location)
		{
			List<ISpyItem> items = new List<ISpyItem>();
			string[] files = Directory.GetFiles(location, "*.ispy"); //Only get files for ispy
			foreach (string file in files)
			{
				StreamReader r = new StreamReader(file);
				ISpyItem newItem = new ISpyItem(location+"\\"+r.ReadLine());
				
				while(r.Peek()!=-1)
				{
					ISpySign newSign = new ISpySign(r.ReadLine()[0], r.ReadLine());
					newItem.signs.Add(newSign);
				}
				r.Close();
				items.Add(newItem);
			}
			return items;
		}
		
		private void randomGame(int _ni, int _nw)
		{
			if (iSpyGames.Count == 0)
			{
				MessageBox.Show("No ISpy games found!");
				return;
			}

			Random ran = new Random();
		    int ni = _ni;
		    int nw = _nw;
            while (ni == _ni && nw == _nw)
            {
                ni = ran.Next(0, iSpyGames.Count);
                nw = ran.Next(0, iSpyGames[ni].signs.Count);
            }
		    SetGame(ni, nw);
		}

        private void SetGame(int ni, int nw)
        {
            setLetter(iSpyGames[ni].signs[nw].startLetter);
            setImage(iSpyGames[ni].image);
            setSign(iSpyGames[ni].signs[nw]);
            CurNI = ni;
            CurNW = nw;
        }

		private void setSign(ISpySign spySign)
		{
			var curSign = Sign.FromName(spySign.signName);
            if (curSign == null)
            {
                throw new Exception("The sign \"" + spySign.signName + "\" has not been trained.");
            }
#if KINECT
            currentSignModel = curSign.SignModel;
            skeletonDrawer = new SkeletonDrawer(SkeletonImage);
            KinectHandler.Get().AddVideoListener(NUIVideoFrameReady);
            CursorController.Get().StartListening();
#else
		    currentSignModel = new MockModel();
#endif
		}

        private void NUIVideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage image = e.ImageFrame.Image;
            CameraImage.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null,
                                                     image.Bits, image.Width * image.BytesPerPixel);
        }

		private void setLetter(char c)
		{
			SpyLetter.Content = c.ToString();
		}

		private void setImage(string file)
		{
			imgISpy.Source = new BitmapImage(new Uri(file));
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
#if KINECT
            CursorController.Get().StopListening();
#endif
			Switcher.Switch(new SelectMode());
		}

		// Get ready to process a sign: this also performs a countdown
		private void StartEvaluating()
		{
			timer.Tick += SignCountDown;
			currentTimerVal = 3;
			lblCountDown.Content = "Get Ready";
			timer.Start();
            hand.Visibility = Visibility.Hidden;
		    lblCountDown.Visibility = Visibility.Visible;
		    lblEntropyLow.Visibility = Visibility.Visible;
            CameraImage.Visibility = Visibility.Visible;
		    imgISpy.Visibility = Visibility.Hidden;
		    btnReady.Visibility = Visibility.Hidden;
#if KINECT
            skeletonDrawer.StartDrawing();
#endif
		}
		
		private void StopTimer()
		{
			timer.Tick -= SignCountDown;
			timer.Stop();
		}

		private void SignCountDown(object sender, EventArgs e)
		{
			if (currentTimerVal < 0)
			{
				lblCountDown.Visibility = Visibility.Hidden;
				StopTimer();
				currentSignModel.EvaluateSequence(ProvideFeedback, LowEntropyHandler, HighEntropyHandler);
			}
			else
			{
                lblCountDown.Content = (currentTimerVal == 0) ? "Start!" : (currentTimerVal).ToString();
				currentTimerVal--;
			}
		}

		private void ProvideFeedback(int skeletonScore, int handScore)
		{
			Switcher.Switch(new ISpyFeedbackView(skeletonScore, handScore, CurNI, CurNW));
		}

		private void LowEntropyHandler(int remainder)
		{
			lblEntropyLow.Content = remainder;
		}

		private void HighEntropyHandler()
		{
			lblEntropyLow.Content = "";
		}

		private void btnReady_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			StartEvaluating();
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

        private void btnReady_MouseEnter(object sender, MouseEventArgs e)
        {
            hand.AnimateEnter(btnReady);
        }

        private void btnBack_MouseEnter(object sender, MouseEventArgs e)
        {
            hand.AnimateEnter(btnBack);
        }
		
		
		
	}
	
	
	
	class ISpyItem
	{
		public string image;
		public List<ISpySign> signs;

		public ISpyItem(string _image)
		{
			image = _image;
			signs = new List<ISpySign>();
		}
	}

	class ISpySign
	{
		public string signName;
		public char startLetter;

		public ISpySign(char _startLetter, string _signName)
		{
			signName = _signName;
			startLetter = _startLetter;
		}
	}
}