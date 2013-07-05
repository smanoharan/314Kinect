using System;
using System.Collections;

namespace FeatureExtraction.Hand
{
    public static class HandImageCleaner
    {
        public static BitArray Clean(BitArray depthMap, int depthWidth, byte[] video, int videoWidth)
        {
            BitArray orig = new BitArray(depthMap);

            // find the border: grow, extract the difference, grow again.
            const int growthSize = 2;
            var grownMap = Dilate(depthMap, depthWidth, growthSize);
            var border = Dilate(depthMap.Xor(grownMap), depthWidth, growthSize + 1);

            // get a more accurate picture by filtering by video
            var filteredBorder = FilterByVideo(border, depthWidth, video, videoWidth);
            var enhancedMap = (filteredBorder == null) ? orig : border.Not().Or(filteredBorder).And(grownMap);
            return RemoveNoise(enhancedMap, depthWidth);
        }

        // 3x3 image close operation (erode, then dilate)
        public static BitArray RemoveNoise(this BitArray map, int width)
        {
            return map.ImageOpen(width, 1).ImageClose(width, 1);
        }

        // image open: erode then dilate
        private static BitArray ImageOpen(this BitArray map, int imageWidth, int crossStructRadius)
        {
            return map
                .Erode(imageWidth, crossStructRadius)
                .Dilate(imageWidth, crossStructRadius);
        }

        // image close: dilate then erode
        private static BitArray ImageClose(this BitArray map, int imageWidth, int crossStructRadius)
        {
            return map
                .Dilate(imageWidth, crossStructRadius)
                .Erode(imageWidth, crossStructRadius);
        }

        /// <summary>
        /// Dilate (or erode) the map by a set amount.
        /// 
        /// This means that each '1' pixel sets all it's neighbours to '1'.
        /// This is done simulataneously for all pixels, as to avoid chaining effects.
        /// </summary>
        /// <param name="map">The original depth map</param>
        /// <param name="width">The width of the depth map</param>
        /// <param name="growthSize">The amount to grow by (in pixels)</param>
        /// <param name="erode">Whether to erode, instead of dilating.</param>
        /// <returns>The resulting map</returns>
        public static BitArray Dilate(this BitArray map, int width, int growthSize, bool erode = false)
        {
            BitArray grownMap = new BitArray(map.Length, erode);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (erode == map[y * width + x]) continue;

                    int minI = Math.Max(0, x - growthSize);
                    int maxI = Math.Min(width - 1, x + growthSize);
                    int minJ = Math.Max(0, y - growthSize);
                    int maxJ = Math.Min(width - 1, y + growthSize);

                    for (int i = minI; i <= maxI; i++)
                    {
                        for (int j = minJ; j <= maxJ; j++)
                        {
                            grownMap[j * width + i] = !erode;
                        }
                    }
                }
            }
            return grownMap;
        }

        public static BitArray Erode(this BitArray map, int width, int growthSize)
        {
            return Dilate(map, width, growthSize, true);
        }

        // use video to find out if these are skin pixels
        // but only use if video is discriminative (at least 20% of each)
        public static BitArray FilterByVideo(this BitArray map, int depthWidth, byte[] video, int videoWidth)
        {
            int positiveCount = 0;
            int totalCount = 0;
            int radius = videoWidth / depthWidth;
            var filteredMap = new BitArray(map.Length);

            // scale by color-max:
            byte maxRGB = 0;
            for (int i = 0; i < videoWidth * videoWidth; i++)
            {
                int j = i * 4;
                maxRGB = Math.Max(Math.Max(maxRGB, video[j]), Math.Max(video[j + 1], video[j + 2]));
            }
            double scale = 255 / (double)maxRGB;

            for (int x = 0; x < depthWidth; x++)
            {
                for (int y = 0; y < depthWidth; y++)
                {
                    int depthIndex = y * depthWidth + x;
                    if (!map[depthIndex]) continue;

                    totalCount++;
                    if (!IsSkin(video, videoWidth, x, y, radius, scale)) continue;

                    positiveCount++;
                    filteredMap[depthIndex] = true;
                }
            }

            // determine the usefulness of the filtering process:
            bool isUsefulFilter = (5 * positiveCount > totalCount && 10 * positiveCount < 9 * totalCount);
            return isUsefulFilter ? filteredMap : null;
        }

        private static bool IsSkin(byte[] video, int videoWidth, int x, int y, int radius, double scale)
        {
            int vI = 4 * (y * 2 * videoWidth + x * 2);
            return SkinGMM.Get().Evaluate(new[] { video[vI + 2] * scale, video[vI + 1] * scale, video[vI] * scale }) >= 0.5;
        }
    }
}
