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
	/// Interaction logic for ispy.xaml
	/// </summary>
	public partial class ispy : UserControl
	{
		public ispy()
		{
			this.InitializeComponent();
			
			setLetter('C');
		}
		
		private void setLetter(char c)
		{
			SpyLetter.Content = c.ToString();
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{			
			Switcher.Switch(new SelectMode());
		}
	}
}