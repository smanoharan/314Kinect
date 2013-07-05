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

namespace SignSelections
{
	/// <summary>
	/// Interaction logic for SignSelection.xaml
	/// </summary>
	public partial class SignSelection : UserControl
	{
		public SignSelection()
		{
			this.InitializeComponent();
		}

        private void ButtonSign16_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Name);
        }

		


       
	}
}