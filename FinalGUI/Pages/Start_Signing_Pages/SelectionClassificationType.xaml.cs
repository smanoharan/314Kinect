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
	/// Interaction logic for SelectionClassificationType.xaml
	/// </summary>
	public partial class SelectionClassificationType : UserControl
	{
	    public SelectionClassificationType()
        {
           
            this.InitializeComponent();
		}

        private void btnListView_Click(object sender, RoutedEventArgs e)
        {
           Switcher.Switch(new SignSelectionListView()); // User chooses 'select by list view'
        }

        private void btnClassifiedView_Click(object sender, RoutedEventArgs e)
        {
			Switcher.Switch(new SignByClassification()); // User chooses 'select by hand shape'
        }

        private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Switcher.Switch(new Menu()); // Go back to main menu
        }
	}
}
