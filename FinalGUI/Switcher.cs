using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;


namespace FinalGUI
{
    public static class Switcher
    {

        public static PageSwitcher pageSwitcher; // The main window that will be swapping the pages

        /// <summary>
        /// Swtich to a new page
        /// </summary>
        /// <param name="newPage">The page to be switched</param>
        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }
        
        /// <summary>
        /// Swtich to a new page givena state
        /// </summary>
        /// <param name="newPage">The page to be switched</param>
        /// <param name="state">The state of the page</param>
        public static void Switch(UserControl newPage, object state)
        {
            pageSwitcher.Navigate(newPage, state);
        }
    }
}
