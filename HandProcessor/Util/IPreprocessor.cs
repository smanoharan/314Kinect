using FeatureExtraction.Skeleton;

namespace FeatureExtraction.Util
{
    /// <summary>
    /// Preprocessing each frame from raw data into a more useful format
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Preprocess a frame, converting raw data (from Kinect), into a 
        ///  preprocessed frame.
        /// </summary>
        /// <param name="skel">The skeleton from Kinect</param>
        /// <param name="video">The raw video output from Kinect</param>
        /// <param name="depth">The raw byte output from Kinect</param>
        /// <returns>A preprocessed frame</returns>
        PreprocessedFrame Preprocess(Skeleton.Skeleton skel, byte[] video, byte[] depth);
    }
}
