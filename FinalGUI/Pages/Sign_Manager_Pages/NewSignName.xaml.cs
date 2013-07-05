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
	/// Interaction logic for NewSignName.xaml
	/// </summary>
	public partial class NewSignName : UserControl
	{

	    private Sign s;

		public NewSignName()
		{
			this.InitializeComponent();
		    s = new Sign();
		}

		public NewSignName(Sign s)
		{
			this.InitializeComponent();
		    this.s = s;
		    txtSignName.Text = s.SignName;
		}
		
		private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (txtSignName.Text != "") // user must specify a sign name to continue
            {
                s.SignName = txtSignName.Text;
                Switcher.Switch(new NewSignPicture(s));
            }
            else
            {
                MessageBox.Show("The sign name cannot be empty.", "Sign name");
            }
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new SignManager());
		}

	}
}