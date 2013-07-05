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
using System.IO;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for SignManager.xaml
	/// </summary>
	public partial class SignManager : UserControl
	{
		private List<string> filePaths;
		
		
		public SignManager()
		{
			this.InitializeComponent();
			filePaths = new List<string>();
		}

		private void btnCreateSign_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new NewSignName());
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new Menu());
		}


		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
            UpdateListBox();
		}
		
        /// <summary>
        /// Grab the filename based on the filepath
        /// </summary>
        /// <param name="filePath">Filepath contraining the filename to be extracted</param>
        /// <returns></returns>
		private string GetFileName(string filePath)
		{
			string[] array;
            array = filePath.Split('\\');
            return array[array.Length - 1];
		}

        private void btnDeleteSign_Click(object sender, RoutedEventArgs e)
        {
            if(listBoxSign.SelectedIndex == -1) // user must select a sign to delete
            {
                MessageBox.Show("Please select a sign in the listbox to delete.", "No signs were selected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (MessageBox.Show("Are you sure that you wish to delete the selected sign?", "Are you sure?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (File.Exists(filePaths[listBoxSign.SelectedIndex]))
                    {
                        File.Delete(filePaths[listBoxSign.SelectedIndex]);
                        UpdateListBox(); // remove the old items in the listbox and re-add the remaining signs
                    }
                }
            }
        }

        private void btnModifySign_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSign.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a sign in the listbox to modify.", "No signs were selected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Switcher.Switch(new SignModifyView(filePaths[listBoxSign.SelectedIndex]));
            }
        }

        /// <summary>
        /// Clears the old listbox by removing all the signs in the textbox
        /// and then re-add the remaining signs.
        /// </summary>
        private void UpdateListBox()
        {
            listBoxSign.Items.Clear();
            foreach (string s in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sign", SearchOption.AllDirectories))
            {
                listBoxSign.Items.Add(GetFileName(s));
                filePaths.Add(s);
            }
        }

	}
}