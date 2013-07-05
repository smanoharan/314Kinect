using FeatureExtraction.Util;

namespace FeatureExtraction.Skeleton
{
    /// <summary>
    /// A data structure representing a processed skeleton.
    /// </summary>
    public class Skeleton
    {
        public Vector3D ShoulderCenter { get; set; }

        public Vector3D ShoulderLeft { get; set; }
        public Vector3D ElbowLeft { get; set; }
        public Vector3D PalmLeft { get; set; }
        public Vector3D WristLeft { get; set; }

        public Vector3D ShoulderRight { get; set; }
        public Vector3D ElbowRight { get; set; }
        public Vector3D PalmRight { get; set; }
        public Vector3D WristRight { get; set; }
    }
}