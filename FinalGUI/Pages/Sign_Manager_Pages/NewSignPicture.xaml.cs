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
using System.Windows.Forms;
using Engine;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for NewSignPicture.xaml
	/// </summary>
	public partial class NewSignPicture : System.Windows.Controls.UserControl
	{
		private Sign s;
		
		public NewSignPicture(Sign s)
		{
			this.InitializeComponent();
			this.s = s;
		    txtPictureURL.Text = s.ImgLocation;
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new NewSignName(s));
		}

		private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
		{
		    s.ImgLocation = txtPictureURL.Text;
			Switcher.Switch(new NewSignClassification(s));
		}

		private void btnUpload_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            OpenFileDialog newDialog = new OpenFileDialog();
            if (newDialog.ShowDialog() == DialogResult.OK)txtPictureURL.Text = newDialog.FileName;
  
		}

	}
}