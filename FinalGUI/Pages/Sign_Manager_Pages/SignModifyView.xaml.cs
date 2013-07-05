using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine;
using Engine.Model;
using Engine.FeatureExtraction;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for SignModifyView.xaml
	/// </summary>
	public partial class SignModifyView : UserControl
	{
		private string filePath; // location of the sign
	    private Sign sign; // sign which stores any change (or no change)
		private List<Sequence> examples; // stores any addition training sequence
		private DispatcherTimer timer; // timer to update process label
		private int processingCounter; // counter used to calcualte the '.'s in the progress
		private AsyncDelegateCommand trainingCommand; // asynchronously build the model
		private bool isTrainingDone = false;
		
		public SignModifyView(string filePath)
		{
			this.InitializeComponent();
			this.filePath = filePath;
			this.examples = null;
		    sign = Sign.FromFile(filePath); // grabs the sign based on the location
			SetValuesInControls(); // set the sign information to the textboxes 
		}
		
		public SignModifyView(Sign s, List<Sequence> examples, string filePath)
		{
			this.InitializeComponent();
			this.filePath = filePath;
			this.examples = examples;
		    sign = s;

            // if new training has been added, show indication (number of ADDITIONAL training)
            if(examples != null)
            {
                if (examples.Count != 0)
                {
                    lblNewExampleCount.Visibility = Visibility.Visible;
                    lblNewExampleCount.Content = examples.Count + " new training examples have been trained";
                }
            }
			SetValuesInControls();
		}
		
		private void SetValuesInControls()
		{
			txtBoxSignName.Text = sign.SignName;
			txtPictureURL.Text = sign.ImgLocation;
			txtVideoURL.Text = sign.VidLocation;
			lblSignClassOutput.Content = sign.SignClass;
		}

		private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new SignManager());
		}

		private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
		{

			if(txtBoxSignName.Text == "")
            {
                MessageBox.Show("The sign name cannot be empty.", "Invalid sign name", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
				MessageBoxResult result = MessageBox.Show("Are you sure that you wish to save changes?", "Are you sure?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
                {
                	File.Delete(filePath);

                    // check if we need to re-train the model
					if(examples == null)
					{
                        // if we don't no need to show processing label

                         UpdateSignTextDetails();
                         Sign.ToFile(sign);
                         Switcher.Switch(new SignManager());
                        
					}
					else
					{
						// Re-train the model, so show processing label
						
						// Hide all the other controls
						lblTitle.Visibility = Visibility.Hidden;
						btnSave.Visibility = Visibility.Hidden;
						btnBack.Visibility = Visibility.Hidden;
						lblSignName.Visibility = Visibility.Hidden;
						txtPictureURL.Visibility = Visibility.Hidden;
						btnPicUpload.Visibility = Visibility.Hidden;
						txtVideoURL.Visibility = Visibility.Hidden;
						btnVidUpload.Visibility = Visibility.Hidden;
						lblPicturePath.Visibility = Visibility.Hidden;
						lblVideoPath.Visibility = Visibility.Hidden;
						lblSignClassOutput.Visibility = Visibility.Hidden;
						lblSignClass.Visibility = Visibility.Hidden;
						txtBoxSignName.Visibility = Visibility.Hidden;
						btnAddExamples.Visibility = Visibility.Hidden;
						btnChangeClass.Visibility = Visibility.Hidden;
						lblNewExampleCount.Visibility = Visibility.Hidden;
						
						// Display processing label as it takes some time to append the training examples
						lblProcessing.Visibility = Visibility.Visible;
						
						this.timer = new DispatcherTimer { IsEnabled = true, Interval = TimeSpan.FromSeconds(1) };
						timer.Tick += OnTimerEvent;
						timer.Start();

						trainingCommand = new AsyncDelegateCommand(action: CreateSignModel);
						trainingCommand.Execute(null);
					}
					
                }
				else if (result == MessageBoxResult.No)
				{
					Switcher.Switch(new SignManager());
				}
            }
			

		}
		
		private void OnTimerEvent(object sender, EventArgs eventArgs)
		{
			if (isTrainingDone)
			{
				timer.Stop();
				timer.Tick -= OnTimerEvent;
				Switcher.Switch(new SignManager());
				return;
			}

            // simulate the process label
			processingCounter++;
			string processingDots = "";
			for(int i = 0; i < processingCounter % 4; i++)
			{
				processingDots += ".";
			}
			
			lblProcessing.Content = "Processing" + processingDots;
		}
		
		public void CreateSignModel()
		{
#if KINECT
			Sign newSign = new Sign(sign.SignName, sign.SignClass, sign.ImgLocation, sign.VidLocation, EngineModel.CreateFromTrainingSequences(examples));
#else
            Sign newSign = new Sign(sign.SignName, sign.SignClass, sign.ImgLocation, sign.VidLocation, null);
#endif
            Sign.ToFile(newSign); // save the new sign
			isTrainingDone = true;
		}

		private void btnPicUpload_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog(txtPictureURL);
		}

		private void btnChangeClass_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            UpdateSignTextDetails();
			Switcher.Switch(new ModifySignClassification(sign, examples, filePath));
		}

		private void btnVidUpload_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OpenFileDialog(txtVideoURL);
		}

		private void btnAddExamples_Click(object sender, System.Windows.RoutedEventArgs e)
		{
		    UpdateSignTextDetails();
			if(examples == null)Switcher.Switch(new ModifySignRecord(sign, filePath));
			else Switcher.Switch(new ModifySignRecord(sign, examples, filePath));
		}

        private void UpdateSignTextDetails()
        {
            sign.SignName = txtBoxSignName.Text;
            sign.ImgLocation = txtPictureURL.Text;
            sign.VidLocation = txtVideoURL.Text;

        }
		
		private void OpenFileDialog(TextBox tb)
		{
			OpenFileDialog newDialog = new OpenFileDialog();

            if (newDialog.ShowDialog() == DialogResult.OK)
            {
                tb.Text = newDialog.FileName;
            }
		}
	}
}