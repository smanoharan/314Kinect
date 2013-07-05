using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for HandController.xaml
	/// </summary>
	public partial class HandController : UserControl
	{
        // Resolution the interface was designed in.
		private const int DesignWidth = 1280; 
		private const int DesignHeight = 800;
		
        private Duration hoverDuration = new Duration(new TimeSpan(0, 0, 2));
        private Duration reverseDuration = new Duration(new TimeSpan(0, 0, 1));
        private DoubleAnimation maskAnimation;

        // Store the button that the hand control hovered on
        private Button hoveredBtn = null;

		public HandController()
		{
			this.InitializeComponent();
		}
		
        /// <summary>
        /// Sets the position of the hand controller
        /// </summary>
        /// <param name="x">X position of the hand control</param>
        /// <param name="y">Y position of the hand control</param>
		public void SetPosition(double x, double y)
        {
            // Sets the (x,y) position to a scale of the user screen.
            Canvas.SetLeft(this, (x / System.Windows.SystemParameters.PrimaryScreenWidth) * DesignWidth);
            Canvas.SetTop(this, (y / System.Windows.SystemParameters.PrimaryScreenHeight) * DesignHeight);
			
        }

        /// <summary>
        /// Start the 'eye opening' animation of the hand control
        /// </summary>
        /// <param name="btn">The button the hand control is hovering</param>
        public void AnimateEnter(Button btn)
        {
            // Set the animation property and duration.
            maskAnimation = new DoubleAnimation(this.Mask.Height, 50, hoverDuration);
            hoveredBtn = btn; // set the button that is being hovered
            maskAnimation.Completed += new EventHandler(maskAnimation_Completed); 
            this.Mask.BeginAnimation(Canvas.HeightProperty, maskAnimation); // begin the animation

        }

        /// <summary>
        /// Start the 'eye shutting' animation of the hand control
        /// </summary>
        public void AnimateLeave()
        {
            // remove previous event
            maskAnimation.Completed -= new EventHandler(maskAnimation_Completed); 
            // set the animation property and duration
            maskAnimation = new DoubleAnimation(this.Mask.Height, 0, reverseDuration);
            hoveredBtn = null; // hand is not hovering anything anymore, so null
            this.Mask.BeginAnimation(Canvas.HeightProperty, maskAnimation); // begin the animation

        }

        /// <summary>
        /// Event which executes upon completion of the hand 'eye opening' animation.
        /// This executes the hovered button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void maskAnimation_Completed(object sender, EventArgs e)
        {
            if (hoveredBtn != null) hoveredBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }


	}
}