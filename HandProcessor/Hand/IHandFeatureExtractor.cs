using System.Collections;

namespace FeatureExtraction.Hand
{
    /// <summary>
    /// Extract the key features from the hand
    /// </summary>
    public interface IHandFeatureExtractor
    {
        /// <summary>
        /// Extract the key features from the raw data into a double array.
        /// </summary>
        /// <param name="video">The cropped video of the hand</param>
        /// <param name="depth">The cropped depth-mask of the hand</param>
        /// <returns>A feature vector representing the hand</returns>
        double[] ExtractFeatures(byte[] video, BitArray depth);
    }
}
