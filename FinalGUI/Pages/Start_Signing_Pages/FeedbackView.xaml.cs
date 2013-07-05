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
using System.Windows.Media.Animation;
using Engine.Model;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for FeedbackView.xaml
	/// </summary>
	public partial class FeedbackView : UserControl
	{

        private Sign sign;

		public FeedbackView(Sign sign, int skeletonScore, int handScore)
		{
            this.InitializeComponent();
			this.Cursor = Cursors.None;
            this.sign = sign;
            ShowFeedback(skeletonScore, handScore);
		}


        private void ShowFeedback(int skeletonScore, int handScore)
        {
            // Display the hand and skeleton score as the feedback
            ScoreFormatter.FormatSkeletonScoreLabel(skeletonScore, cirImage, lblTotalResult);
            ScoreFormatter.FormatHandLabel(handScore, lblHandResult);
        }

		private void btnDone_Click(object sender, System.Windows.RoutedEventArgs e)
		{
#if KINECT
            CursorController.Get().StopListening();
#endif
            Switcher.Switch(new SignSelectionListView());
		}

		private void btnTryAgain_Click(object sender, System.Windows.RoutedEventArgs e)
        {
#if KINECT
            CursorController.Get().StopListening();
#endif
            Switcher.Switch(new PerformView(sign));
		}
		
		
		
		/***********************************Kinect Hand controller**********************************************/
	
		private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.SetPosition(e.GetPosition(this).X, e.GetPosition(this).Y);
		}

		private void btnDone_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnDone);
		}

		private void btnTryAgain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnTryAgain);
		}

		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
            hand.AnimateLeave();
		}






		

	}
}