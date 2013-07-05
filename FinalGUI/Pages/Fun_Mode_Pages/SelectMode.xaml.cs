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
	/// Interaction logic for SelectMode.xaml
	/// </summary>
	public partial class SelectMode : UserControl
	{
		public SelectMode()
		{
			this.InitializeComponent();
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new Menu());
		}

		private void btnISpy_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            Sign.UpdateSignList();
            try
            {
                Switcher.Switch(new ISpyPlay());
            }
			catch(Exception ex)
			{
			    MessageBox.Show(ex.Message);
			}
		}

		private void btnNurRhy_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new StoryManagerView());
		}
	}
}