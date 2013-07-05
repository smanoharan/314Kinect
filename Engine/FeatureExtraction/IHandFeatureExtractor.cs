using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.FeatureExtraction
{
    public interface IHandFeatureExtractor
    {
        double[] ExtractFeatures(byte[] video, BitArray depth);
    }
}
