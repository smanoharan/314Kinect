using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Engine;
using Engine.Model;
using Engine.Story;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for FeedbackView.xaml
	/// </summary>
	public partial class StoryFeedbackView : UserControl
	{
		private Sign sign;
		private LinkedList<Scenario>.Enumerator iter;

		public StoryFeedbackView(Sign sign, int skeletonScore, int handScore, LinkedList<Scenario>.Enumerator iter)
		{
			InitializeComponent();
			this.Cursor = Cursors.None;
		
			this.iter = iter;
			this.sign = sign;
			ShowFeedback(skeletonScore, handScore);

			if (skeletonScore < 50)
			{
			    btnDone.Visibility = Visibility.Hidden;
			}
			
		}


		private void ShowFeedback(int skeletonScore, int handScore)
		{
            ScoreFormatter.FormatSkeletonScoreLabel(skeletonScore, cirImage, lblTotalResult);
            ScoreFormatter.FormatHandLabel(handScore, lblHandResult);

            if (skeletonScore < 30) btnDone.Visibility = Visibility.Hidden;
		}

		private void btnDone_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!iter.MoveNext())
			{
				Switcher.Switch(new FinalComplete());
				//Switcher.Switch(new StoryManagerView());
			}
			else Switcher.Switch(new StoryPerformView(iter));
		}

		private void btnTryAgain_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new StoryPerformView(iter));
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

		private void btnDone_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnDone);
		}

		private void btnTryAgain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnTryAgain);
		}

	}
}