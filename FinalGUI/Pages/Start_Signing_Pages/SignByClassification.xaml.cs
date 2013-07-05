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
	/// Interaction logic for SignByClassification.xaml
	/// </summary>
	public partial class SignByClassification : UserControl
	{
		public SignByClassification()
		{
			this.InitializeComponent();
		}

        private void HandClassClick(object sender, RoutedEventArgs e)
        {
            // Choose a sign base on the hand shape selected
            HandShapes hs = (HandShapes)Enum.Parse(typeof(HandShapes), ((Button) sender).Name);
            Switcher.Switch(new SignSelectionListView(hs));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new SelectionClassificationType());
        }



	}
}