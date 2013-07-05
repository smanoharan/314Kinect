using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalGUI
{
    static class ScoreFormatter
    {
        // Set the brushes that will be used for the hand score.
        private static readonly Brush goodScoreColorBrush = new SolidColorBrush(Colors.Green);
        private static readonly Brush mediumScoreColorBrush = new SolidColorBrush(Colors.Yellow);
        private static readonly Brush badScoreColorBrush = new SolidColorBrush(Colors.DarkOrange);

        /// <summary>
        /// Set the hand score with the appropriate colour brush
        /// </summary>
        /// <param name="handScore">Hand score from the performance</param>
        /// <param name="handLabel">Label which contains that hand score</param>
        public static void FormatHandLabel(int handScore, Label handLabel)
        {
            int gHandScore = handScore / 10;
            if (gHandScore <= 1)
            {
                handLabel.Content = "";
                handLabel.Foreground = badScoreColorBrush;
            }
            else if (gHandScore <= 4)
            {
                handLabel.Content = "Good Hand Shape";
                handLabel.Foreground = mediumScoreColorBrush;
            }
            else
            {
                handLabel.Content = "Excellent Hand Shape!";
                handLabel.Foreground = goodScoreColorBrush;
            }
        }

        /// <summary>
        /// Sets the skeleton score, smilely face based on the given output control.
        /// </summary>
        /// <param name="skelScore">The skeleton score of the performance</param>
        /// <param name="faceEllipse">Ellipse that will hold the smiley image</param>
        /// <param name="lblTotalResult">Label that contains the score</param>
        public static void FormatSkeletonScoreLabel(int skelScore, Ellipse faceEllipse, Label lblTotalResult)
        {
            var myBrush = new ImageBrush();
            if (skelScore / 10 >= 8) myBrush.ImageSource = new BitmapImage(new Uri("../../Images/face5.png", UriKind.Relative));
            else if (skelScore / 10 >= 7) myBrush.ImageSource = new BitmapImage(new Uri("../../Images/face4.png", UriKind.Relative));
            else if (skelScore / 10 >= 5) { myBrush.ImageSource = new BitmapImage(new Uri("../../Images/face3.png", UriKind.Relative)); }
            else if (skelScore / 10 >= 3) myBrush.ImageSource = new BitmapImage(new Uri("../../Images/face2.png", UriKind.Relative));
            else myBrush.ImageSource = new BitmapImage(new Uri("../../Images/face1.png", UriKind.Relative));
            faceEllipse.Fill = myBrush;

            lblTotalResult.Content = "Total Score: " + (skelScore / 10) + "/10";
        }
    }
}
