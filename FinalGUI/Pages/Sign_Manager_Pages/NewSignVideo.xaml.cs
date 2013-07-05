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
using Engine.Model;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for NewSignVideo.xaml
	/// </summary>
	public partial class NewSignVideo : System.Windows.Controls.UserControl
	{
		private Sign s;

        public NewSignVideo(Sign s)
		{
			this.InitializeComponent();
            this.s = s;
            txtVideoURL.Text = s.VidLocation;
		}
		

		private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
		{
		    s.VidLocation = txtVideoURL.Text;
			Switcher.Switch(new NewSignRecord(s));
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{

			Switcher.Switch(new NewSignClassification(s));
		}

		private void btnUpload_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog newDialog = new OpenFileDialog();
            if (newDialog.ShowDialog() == DialogResult.OK) txtVideoURL.Text = newDialog.FileName;
            
		}
	}
}