using System;
using System.Collections.Generic;
using System.Text;
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
using Engine.Story;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for SignManager.xaml
	/// </summary>
	public partial class StoryManagerView : UserControl
	{
		private readonly List<string> filePaths;

		public StoryManagerView()
		{
			this.InitializeComponent();
			filePaths = new List<string>();
		}

		private void btnCreateSign_Click(object sender, System.Windows.RoutedEventArgs e)
		{
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new SelectMode());
		}


		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
	  
			foreach(string s in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.story", SearchOption.AllDirectories))
			{
				listBoxStories.Items.Add(GetFileName(s));
				filePaths.Add(s);
			}
		}
		
		private string GetFileName(string filePath)
		{
			return filePath.Split('\\').Last();
		}

		private void btnDeleteSign_Click(object sender, RoutedEventArgs e)
		{
			if(listBoxStories.SelectedIndex == -1)
			{
				MessageBox.Show("Please select a story in the listbox to delete.", "No story was selected", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				if (MessageBox.Show("Are you sure you wish to delete the selected story?", "Are you sure?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					if (File.Exists(filePaths[listBoxStories.SelectedIndex]))
						File.Delete(filePaths[listBoxStories.SelectedIndex]);
				}
			}
		}

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			if (listBoxStories.SelectedIndex == -1)
			{
				MessageBox.Show("Please select a story in the listbox to play.", "No story was selected", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				var selectedFile = filePaths[listBoxStories.SelectedIndex];
				var fileStream = new FileStream(selectedFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				var story = Scenario.FromFile(fileStream);
				fileStream.Close();
				var it = story.GetEnumerator();
				it.MoveNext();
				Switcher.Switch(new StoryPerformView(it));
			}
		}

		private void btnNewStory_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			
			// TODO ... Editor
			//Switcher.Switch(new NewSignName());
		}

	}
}