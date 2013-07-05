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

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for FeedbackView.xaml
	/// </summary>
	public partial class ISpyFeedbackView : UserControl
	{
	    private int nw;
	    private int ni;

	    public ISpyFeedbackView(int skeletonScore, int handScore, int ni, int nw)
		{
	        this.nw = nw;
	        this.ni = ni;
	        this.InitializeComponent();
			this.Cursor = Cursors.None;

            ShowFeedback(skeletonScore, handScore);
		}


        private void ShowFeedback(int skeletonScore, int handScore)
        {
            ScoreFormatter.FormatSkeletonScoreLabel(skeletonScore, cirImage, lblTotalResult);
            ScoreFormatter.FormatHandLabel(handScore, lblHandResult);
            if (skeletonScore < 30) 
                btnNext.Visibility = Visibility.Hidden;
        }



        private void btnTryAgain_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Switcher.Switch(new ISpyPlay(ni, nw, true));
            }
            catch (Exception ex)
            {
#if KINECT
                CursorController.Get().StopListening();
#endif
                MessageBox.Show(ex.Message);
                Switcher.Switch(new SelectMode());
            }
        }

        private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	try
            {
                Switcher.Switch(new ISpyPlay(ni, nw, false));
            }
            catch (Exception ex)
            {
#if KINECT
                CursorController.Get().StopListening();
#endif
                MessageBox.Show(ex.Message);
                Switcher.Switch(new SelectMode());
            }
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

        private void btnTryAgain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            hand.AnimateEnter(btnTryAgain);
        }

        private void btnNext_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            hand.AnimateEnter(btnNext);
        }
	}
}