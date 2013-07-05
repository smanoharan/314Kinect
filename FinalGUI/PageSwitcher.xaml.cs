using System;
using System.Windows;
using System.Windows.Forms;
using Engine.Kinect;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace FinalGUI
{
	/// <summary>
	/// Interaction logic for PageSwitcher.xaml
	/// </summary>
	public partial class PageSwitcher : Window
	{
		public PageSwitcher()
		{
			InitializeComponent();
            /* set this window as the main window to deal with the switching 
              of the pages */
			Switcher.pageSwitcher = this; 
			Switcher.Switch(new Menu()); // the menu page is the beginning page, so load this.
#if KINECT
			try
		    {
		        KinectHandler.Get();
		    } 
            catch
		    {
		        MessageBox.Show("Please plug in your Kinect to continue. Exiting.");
                Environment.Exit(0);
		    }
		    //var d = Screen.PrimaryScreen.Bounds;
		    //CursorController.Get().StartListening(d.Width, d.Height);
#endif
		}

        /// <summary>
        /// Set the content of the window from a given page
        /// </summary>
        /// <param name="nextPage">Page containing the content to load</param>
		public void Navigate(UserControl nextPage)
		{
			this.Content = nextPage;
		}
		
        /// <summary>
        /// Set the content of the window from a given usercontrol
        /// and a given state
        /// </summary>
        /// <param name="nextPage">Page containing the content to load</param>
        /// <param name="state">State of the page</param>
		public void Navigate(UserControl nextPage, object state)
		{
			this.Content = nextPage;
			ISwitchable s = nextPage as ISwitchable;

			if (s != null)
				s.UtilizeState(state);
			else
				throw new ArgumentException("NextPage is not ISwitchable! "
				  + nextPage.Name.ToString());
		}

	}
}
