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
using Engine.FeatureExtraction;
using Engine;
using Engine.Model;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for ModifySignClassification.xaml
	/// </summary>
	public partial class ModifySignClassification : UserControl
	{
	    private Sign s; // current sign which contains all the updates
		private string filePath; // filepath of the sign to be modified
	    private List<Sequence> examples; // list of any new training examples
		
		public ModifySignClassification(Sign s, List<Sequence> examples, string filePath)
		{
			this.InitializeComponent();

		    this.s = s;
			this.filePath = filePath;
		    this.examples = examples;
		}
		

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new SignModifyView(s, examples, filePath));
		}

		private void HandClassClick(object sender, System.Windows.RoutedEventArgs e)
		{
			HandShapes hs = (HandShapes)Enum.Parse(typeof(HandShapes), ((Button) sender).Name);
			s.SignClass = hs;
            Switcher.Switch(new SignModifyView(s, examples, filePath)); // updates the sign hand shape class
		}
	}
}