using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Engine;
using Engine.Model;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for SignSelectionListView.xaml
	/// </summary>
	public partial class SignSelectionListView : UserControl
	{
        /// <summary>
        /// SignFile stores the location of the sign and name
        /// </summary>
        class SignFile
        {
            private readonly string displayName;
            public string Path { get; set; }

            public SignFile(string filePath)
            {
                displayName = filePath.Split('\\').Last();
                Path = filePath;
            }

            public override string ToString()
            {
                return displayName;
            }
        }

        public SignSelectionListView()
		{
			this.InitializeComponent();

            // Gets all the sign from the application directory.
            foreach (var s in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sign", SearchOption.AllDirectories))
            {
                listBoxSign.Items.Add(new SignFile(s));
            }
		}

        public SignSelectionListView(HandShapes hs)
        {
            this.InitializeComponent();

            // Gets all the signs from a given 'hand shape' directory
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\" + hs))
            {
                foreach (
                    var s in
                        Directory.GetFiles(Directory.GetCurrentDirectory() + "\\" + hs, "*.sign",
                                           SearchOption.AllDirectories))
                {
                    listBoxSign.Items.Add(new SignFile(s));
                }
            }
        }


		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new SelectionClassificationType());
		}

		private void btnNext_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (listBoxSign.SelectedIndex != -1)
                Switcher.Switch(new PreviewView(Sign.FromFile(((SignFile)listBoxSign.SelectedItem).Path)));
            else
                MessageBox.Show("You must select a sign in the list box", "Please select a sign");    
		}

        /// <summary>
        /// Grabs the filename only from a given file path
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns></returns>
        private string GetFileName(string filePath)
        {
            string[] array;
            array = filePath.Split('\\');
            return array[array.Length - 1];
        }
	}
}