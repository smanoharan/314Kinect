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

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for NewSignComplete.xaml
	/// </summary>
	public partial class NewSignComplete : UserControl
	{
		public NewSignComplete()
		{
			this.InitializeComponent();
			this.Cursor = Cursors.None;
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
#if KINECT
            CursorController.Get().StopListening();
#endif
            Switcher.Switch(new SignManager());
		}

		
		/************************************Kinect Controller *******************************/
		
		private void btnBack_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateEnter(btnBack);
		}

		private void btnBack_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.AnimateLeave();
		}

		private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			hand.SetPosition(e.GetPosition(this).X, e.GetPosition(this).Y);
		}
		

	}
}