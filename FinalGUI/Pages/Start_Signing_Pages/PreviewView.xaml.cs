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
using Engine.Model;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for PreviewView.xaml
	/// </summary>
	public partial class PreviewView : UserControl
	{
        private Sign sign;

		public PreviewView(Sign sign)
		{
			this.InitializeComponent();

            this.sign = sign;

            lblSignName.Content = sign.SignName; // loads the sign name
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri(sign.ImgLocation, UriKind.Relative));
            imgSign.Fill = myBrush; // Set the image picture for the sign
            mediaElement.Source = new Uri(sign.VidLocation, UriKind.Relative); // set the video for the sign
		}


		private void MediaPlayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//MediaPlayer.Position = TimeSpan.Zero;
        	//MediaPlayer.Play();
		}

        private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Switcher.Switch(new SignSelectionListView());
        }

        private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Switcher.Switch(new PerformView(sign));
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Source = new Uri(sign.VidLocation, UriKind.Relative);
        }


	}
}