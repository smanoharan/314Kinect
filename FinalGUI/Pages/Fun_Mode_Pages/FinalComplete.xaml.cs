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

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for FinalComplete.xaml
	/// </summary>
	public partial class FinalComplete : UserControl
	{
		public FinalComplete()
		{
			this.InitializeComponent();
			this.Cursor = Cursors.None;
            cirImage.Fill = new ImageBrush(new BitmapImage(new Uri("../../Images/face5.png", UriKind.Relative)));
		}
        
		private void btnMainMenu_Click(object sender, System.Windows.RoutedEventArgs e)
		{	
#if KINECT
            CursorController.Get().StopListening();
#endif
			Switcher.Switch(new SelectMode());	
		}

		

		private void btnNextStory_Click(object sender, System.Windows.RoutedEventArgs e)
		{
#if KINECT
            CursorController.Get().StopListening();
#endif
			Switcher.Switch(new StoryManagerView());
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
		
		private void btnNextStory_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnNextStory);
		}

		private void btnMainMenu_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnMainMenu);
		}
	}
}