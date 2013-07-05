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
	/// Interaction logic for NewSignClassification.xaml
	/// </summary>
	public partial class NewSignClassification : UserControl
	{
	    private Sign s;
		
		public NewSignClassification(Sign s)
		{
			this.InitializeComponent();
		    this.s = s;
		}
		
		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new NewSignPicture(s));
		}

		private void HandClassClick(object sender, System.Windows.RoutedEventArgs e)
		{
            // grab the correct HandShapes Enum
			HandShapes hs = (HandShapes)Enum.Parse(typeof(HandShapes), ((Button) sender).Name);
			s.SignClass = hs;
            Switcher.Switch(new NewSignVideo(s));
		}

	}
}
