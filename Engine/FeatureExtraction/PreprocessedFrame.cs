using System.Collections;

namespace Engine.FeatureExtraction
{
    public class PreprocessedFrame
    {
        public double[] SkeletonFeatures { get; private set; }
        public byte[] CroppedVideoLeft { get; private set; }
        public byte[] CroppedVideoRight { get; private set; }
        public BitArray DepthMapLeft { get; private set; }
        public BitArray DepthMapRight { get; private set; }

        public PreprocessedFrame(double[] skeletonFeatures,
            byte[] croppedVideoLeft, byte[] croppedVideoRight, BitArray depthMapLeft, BitArray depthMapRight)
        {
            SkeletonFeatures = skeletonFeatures;
            CroppedVideoLeft = croppedVideoLeft;
            CroppedVideoRight = croppedVideoRight;
            DepthMapLeft = depthMapLeft;
            DepthMapRight = depthMapRight;
        }

    }
}
