namespace FeatureExtraction.Util
{
    public static class CropUtil
    {
        /// <summary>
        /// Crop an image (either a depth-map or a video image) to a square 
        ///   centered on the hand position.
        /// 
        /// The image must be represented as a byte[]. 
        /// This byte[] is interpreted as 3D matrix ( width * height * depth) where
        ///     width is the width of the image, e.g. 640px
        ///     height is the height of the image, e.g. 480px
        ///     depth is the number of bytes per pixel, e.g. 4 (=32 bits) in the bgr32 format.
        /// </summary>
        /// <param name="handPosX">The x-coordinate of the hand</param>
        /// <param name="handPosY">The y-coordinate of the hand</param>
        /// <param name="cropRadius">The side length of the square to crop</param>
        /// <param name="orig">The original image</param>
        /// <param name="origHeight">The height of the original image</param>
        /// <param name="origWidth">The width of the original image</param>
        /// <param name="elemSize">The size of each pixel, in bytes (4 for bgr32, 2 for depth)</param>
        /// <returns>The cropped image, centered at the hand</returns>
        public static byte[] CropImageToHandPosition(int handPosX, int handPosY, int cropRadius,
            byte[] orig, int origHeight, int origWidth, int elemSize)
        {
            int startX, startY;
            return CropImageToHandPosition(handPosX, handPosY, cropRadius, orig, origHeight, origWidth, elemSize, out startX, out startY);
        }

        /// <summary>
        /// Crop an image (either a depth-map or a video image) to a square 
        ///   centered on the hand position.
        /// 
        /// The image must be represented as a byte[]. 
        /// This byte[] is interpreted as 3D matrix ( width * height * depth) where
        ///     width is the width of the image, e.g. 640px
        ///     height is the height of the image, e.g. 480px
        ///     depth is the number of bytes per pixel, e.g. 4 (=32 bits) in the bgr32 format.
        /// 
        /// Note: Since the square surrounding the hand is bounded to the original image, 
        ///     it is possible that the hand is not centered at the cropped image.
        /// 
        /// The top-left position of the chosen cropped image is returned in output variables
        ///     StartX and StartY.
        /// </summary>
        /// <param name="handPosX">The x-coordinate of the hand</param>
        /// <param name="handPosY">The y-coordinate of the hand</param>
        /// <param name="cropRadius">The half-length of the square to crop</param>
        /// <param name="orig">The original image</param>
        /// <param name="origHeight">The height of the original image</param>
        /// <param name="origWidth">The width of the original image</param>
        /// <param name="elemSize">The size of each pixel, in bytes (4 for bgr32, 2 for depth)</param>
        /// <param name="startX">
        ///     The x-coordinate of the top left corner of the cropped image in the original image
        /// </param>
        /// <param name="startY">
        ///     The y-coordinate of the top left corner of the cropped image in the original image
        /// </param>
        /// <returns>The cropped image, centered at the hand</returns>
        public static byte[] CropImageToHandPosition(int handPosX, int handPosY, int cropRadius,
            byte[] orig, int origHeight, int origWidth, int elemSize, out int startX, out int startY)
        {
            startX = handPosX - cropRadius;
            startY = handPosY - cropRadius;
            int diameter = 2 * cropRadius;

            // make sure that the start position is inside the original image:
            if (startX < 0) startX = 0;
            else if (startX + diameter >= origWidth) startX = origWidth - diameter;

            if (startY < 0) startY = 0;
            else if (startY + diameter >= origHeight) startY = origHeight - diameter;

            return Crop(orig, origWidth, elemSize, diameter, diameter, startX, startY);
        }

        /// <summary>
        /// Crop a byte[], representing a 3D matrix of bytes:
        ///     i.e. 2D matrix of pixels (i.e. by location) 
        ///     where each pixel occupies elemSize bytes.
        /// </summary>
        /// <param name="orig">The original image</param>
        /// <param name="origWidth">The width of the original image</param>
        /// <param name="elemSize">The size of each pixel, in bytes (4 for bgr32, 2 for depth)</param>
        /// <param name="croppedWidth">The desired width of the cropped image</param>
        /// <param name="croppedHeight">The desired height of the cropped image</param>
        /// <param name="startX">The left-most boundary of the cropped image</param>
        /// <param name="startY">The top-most boundary of the cropped image</param>
        /// <returns>A byte[] representing the cropped image</returns>
        public static byte[] Crop(byte[] orig, int origWidth, int elemSize, int croppedWidth, int croppedHeight, int startX, int startY)
        {
            // assign room for the cropped image
            var croppedImage = new byte[elemSize * croppedHeight * croppedWidth];
            
            // iterate through the original image, one row at a time, copying over the relevant pixels.
            int croppedIndex = 0;
            for (int curRow = 0; curRow < croppedHeight; curRow++)
            {
                int startIndex = ToIndex(startX, curRow + startY, origWidth, elemSize);
                for (int i = 0; i < croppedWidth * elemSize; i++)
                    croppedImage[croppedIndex++] = orig[startIndex + i];
            }
            return croppedImage;
        }

        // convert (i,j)*elemSize index to a index in byte[].
        private static int ToIndex(int curX, int curY, int origWidth, int elemSize)
        {
            return elemSize * (curY * origWidth + curX);
        }
    }
}
