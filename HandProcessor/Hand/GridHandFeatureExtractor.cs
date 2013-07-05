using System.Collections;

namespace FeatureExtraction.Hand
{
    public class GridHandFeatureExtractor : IHandFeatureExtractor
    {
        const int Width = 64;
        const int SqrtRegions = 4;
        const int RegionThreshold = 25;
        const int RegionWidth = Width / SqrtRegions;

        /// <summary>
        /// Extract the key features from the raw data into a double array.
        /// </summary>
        /// <param name="video">The cropped video of the hand</param>
        /// <param name="depth">The cropped depth-mask of the hand</param>
        /// <returns>A feature vector representing the hand</returns>
        public virtual double[] ExtractFeatures(byte[] video, BitArray depth)
        {
            // clean up the hand image, then extract grid-based features
            var cleanBitMap = HandImageCleaner.Clean(depth, Width, video, Width * 2);
            return GroupByGrid(cleanBitMap, SqrtRegions, RegionWidth, RegionThreshold);
        }

        /// <summary>
        /// Extract Features from a depth mask using a grid based pool method.
        /// 
        /// The regions which have a pixel count greater than or equal to the threshold 
        ///     have a feature value of 1. All others are 0.
        /// </summary>
        /// <param name="depthMask">
        ///     The mask of the hand, where 1 represents a pixel which is part of the hand
        /// </param>
        /// <param name="sqrtRegions">
        ///     The square root of the number of regions to split into. This forces the grid to be a square.
        /// </param>
        /// <param name="regionWidth">The width (in pixels) of each region</param>
        /// <param name="regionThreshold">The threshold for whether a region is considered to be 1 or a 0 val</param>
        /// <returns>A feature array where each bit represents a specific region</returns>
        public static double[] GroupByGrid(BitArray depthMask, int sqrtRegions, int regionWidth, int regionThreshold)
        {
            // the number of features = number of regions
            int regions = sqrtRegions*sqrtRegions;
            int width = sqrtRegions*regionWidth;
            var feats = new double[regions];

            for (int i = 0; i < sqrtRegions; i++)
            {
                for (int j = 0; j < sqrtRegions; j++)
                {
                    int iStart = i * regionWidth;
                    int jStart = j * regionWidth;
                    int regionIndex = i * sqrtRegions + j;

                    // sum the number of hand pixels over the region
                    int sum = 0;
                    for (int x = 0; x < regionWidth; x++)
                        for (int y = 0; y < regionWidth; y++)
                            if (depthMask.Get((y + jStart) * width + (x + iStart)))
                                sum++;

                    // this feature is 1 iff number of hand pixels exceeds the threshold
                    feats[regionIndex] = sum < regionThreshold ? 0 : 1;
                }
            }

            return feats;
        }
    }
}