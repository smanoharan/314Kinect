namespace Engine.FeatureExtraction
{
    /// <summary>
    /// Represents a sequence of extracted skeleton and hand features
    /// </summary>
    public class Sequence
    {
        public double[][] SkeletonFeatures { get; private set; }
        public double[][] HandFeatures { get; private set; }

        public Sequence(double[][] skelFeats, double[][] handFeats)
        {
            SkeletonFeatures = skelFeats;
            HandFeatures = handFeats;
        }
    }
}
