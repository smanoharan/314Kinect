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
	/// Main window view
	/// </summary>
	public partial class Menu : UserControl
	{
		public Menu()
		{
			this.InitializeComponent();
		}


		private void btnQuit_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Application.Current.Shutdown(0); // exits the application
		}

		private void btnStartSigning_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            // Go to start signing view
			Switcher.Switch(new SelectionClassificationType());
		}

		private void btnCreateSign_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            // Go to Sign manager view
			Switcher.Switch(new SignManager());
		}

		private void btnFunMode_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            // Go to mini game view
			Switcher.Switch(new SelectMode());
		}
    }
}