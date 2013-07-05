namespace Engine.FeatureExtraction
{
    public static class CropUtil
    {
        public static byte[] CropImage(int handPosX, int handPosY, int radius,
            byte[] orig, int origHeight, int origWidth, int elemSize)
        {
            int startX, startY;
            return CropImage(handPosX, handPosY, radius, orig, origHeight, origWidth, elemSize, out startX, out startY);
        }

        public static byte[] CropImage(int handPosX, int handPosY, int radius,
            byte[] orig, int origHeight, int origWidth, int elemSize, out int startX, out int startY)
        {
            startX = handPosX - radius;
            startY = handPosY - radius;
            int diameter = 2 * radius;

            if (startX < 0) startX = 0;
            else if (startX + diameter >= origWidth) startX = origWidth - diameter;

            if (startY < 0) startY = 0;
            else if (startY + diameter >= origHeight) startY = origHeight - diameter;

            return Crop(orig, origWidth, origHeight, elemSize, diameter, diameter, startX, startY);
        }

        public static byte[] Crop(byte[] orig, int origWidth, int origHeight, int elemSize,
                                  int croppedWidth, int croppedHeight, int startX, int startY)
        {
            var croppedImage = new byte[elemSize * croppedHeight * croppedWidth];
            int croppedIndex = 0;
            for (int curRow = 0; curRow < croppedHeight; curRow++)
            {
                int startIndex = ToIndex(startX, curRow + startY, origWidth, elemSize);
                for (int i = 0; i < croppedWidth * elemSize; i++)
                    croppedImage[croppedIndex++] = orig[startIndex + i];
            }
            return croppedImage;
        }

        private static int ToIndex(int curX, int curY, int origWidth, int elemSize)
        {
            return elemSize * (curY * origWidth + curX);
        }
    }
}
