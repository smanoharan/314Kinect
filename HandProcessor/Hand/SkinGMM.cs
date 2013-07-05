using Accord.Statistics.Distributions.Multivariate;

namespace FeatureExtraction.Hand
{
    /// <summary>
    /// GMM (Gaussian Mixture Model) for skin pixel detection: 
    /// 
    /// Models are initialised using parameters given by: 
    ///     M. J. Jones and J. M. Rehg. 
    ///     “Statistical Color Models with Application to Skin Detection.” 
    ///     IEEE Computer Society Conference on Computer Vision and Pattern Recognition, 1999.
    /// 
    /// </summary>
    public class SkinGMM
    {
        // the model has 16 kernels for both the skin and the non-skin version.
        private const int Components = 16;
        private readonly Mixture<NormalDistribution> positive;
        private readonly Mixture<NormalDistribution> negative;

        // singleton:
        private static SkinGMM inst;
        public static SkinGMM Get()
        {
            return inst ?? (inst = new SkinGMM());
        }

        private SkinGMM()
        {
            // load the +ve and -ve GMMs:
            negative = CreateNegativeSkinModel();
            positive = CreatePositiveSkinModel();
        }

        /// <summary>
        /// Calculate the probability that a given colour 
        ///     (represented as a vector of doubles, in [0,1] range)
        ///     is part of skin. 
        /// </summary>
        /// <param name="colour">The colour of the pixel, represented as a vector of doubles</param>
        /// <returns>The probability that the pixel is part of skin</returns>
        public double Evaluate(double[] colour)
        {
            double positiveLikelihood = positive.ProbabilityDensityFunction(colour);
            double negativeLikelihood = negative.ProbabilityDensityFunction(colour);

            // convert to probability (in [0,1] space) by normalising:
            return positiveLikelihood / (positiveLikelihood + negativeLikelihood);
        }

        // helper function for creating a distribution from known parameters
        private static NormalDistribution ToDistribution(
            double meanRed, double meanGreen, double meanBlue,
            double covarRedRed, double covarGreenGreen, double covarBlueBlue)
        {
            var means = new[] { meanRed, meanGreen, meanBlue };
            var covar = new[,] { { covarRedRed, 0, 0 }, { 0, covarGreenGreen, 0 }, { 0, 0, covarBlueBlue } };
            return new NormalDistribution(means, covar);
        }

        private static Mixture<NormalDistribution> CreateNegativeSkinModel()
        {
            var distributions = new NormalDistribution[Components];

            // copy the parameters of the kernels, verbatim, from the paper mentioned above.
            distributions[0] = ToDistribution(73.53, 29.94, 17.76, 765.40, 121.44, 112.80);
            distributions[1] = ToDistribution(249.71, 233.94, 217.49, 39.94, 154.44, 396.05);
            distributions[2] = ToDistribution(161.68, 116.25, 96.95, 291.03, 60.48, 162.85);
            distributions[3] = ToDistribution(186.07, 136.62, 114.40, 274.95, 64.60, 198.27);
            distributions[4] = ToDistribution(189.26, 98.37, 51.18, 633.18, 222.40, 250.69);
            distributions[5] = ToDistribution(247.00, 152.20, 90.84, 65.23, 691.53, 609.92);
            distributions[6] = ToDistribution(150.10, 72.66, 37.76, 408.63, 200.77, 257.57);
            distributions[7] = ToDistribution(206.85, 171.09, 156.34, 530.08, 155.08, 572.79);
            distributions[8] = ToDistribution(212.78, 152.82, 120.04, 160.57, 84.52, 243.90);
            distributions[9] = ToDistribution(234.87, 175.43, 138.94, 163.80, 121.57, 279.22);
            distributions[10] = ToDistribution(151.19, 97.74, 74.59, 425.40, 73.56, 175.11);
            distributions[11] = ToDistribution(120.52, 77.55, 59.82, 330.45, 70.34, 151.82);
            distributions[12] = ToDistribution(192.20, 119.62, 82.32, 152.76, 92.14, 259.15);
            distributions[13] = ToDistribution(214.29, 136.08, 87.24, 204.90, 140.17, 270.19);
            distributions[14] = ToDistribution(99.57, 54.33, 38.06, 448.13, 90.18, 151.29);
            distributions[15] = ToDistribution(238.88, 203.08, 176.91, 178.38, 156.27, 404.99);

            var proportions = new[] 
            {   
                0.0294, 0.0331, 0.0654, 0.0756, 0.0554, 0.0314, 0.0454, 0.0469, 
                0.0956, 0.0763, 0.1108, 0.0676, 0.0755, 0.0500, 0.0667, 0.0749
            };

            return new Mixture<NormalDistribution>(proportions, distributions);
        }

        private static Mixture<NormalDistribution> CreatePositiveSkinModel()
        {
            var distributions = new NormalDistribution[Components];

            // copy the parameters of the kernels, verbatim, from the paper mentioned above.
            distributions[0] = ToDistribution(254.37, 254.41, 253.82, 2.77, 2.81, 5.46);
            distributions[1] = ToDistribution(9.39, 8.09, 8.52, 46.84, 33.59, 32.48);
            distributions[2] = ToDistribution(96.57, 96.95, 91.53, 280.69, 156.79, 436.58);
            distributions[3] = ToDistribution(160.44, 162.49, 159.06, 355.98, 115.89, 591.24);
            distributions[4] = ToDistribution(74.98, 63.23, 46.33, 414.84, 245.95, 361.27);
            distributions[5] = ToDistribution(121.83, 60.88, 18.31, 2502.24, 1383.53, 237.18);
            distributions[6] = ToDistribution(202.18, 154.88, 91.04, 957.42, 1766.94, 1582.52);
            distributions[7] = ToDistribution(193.06, 201.93, 206.55, 562.88, 190.23, 447.28);
            distributions[8] = ToDistribution(51.88, 57.14, 61.55, 344.11, 191.77, 433.40);
            distributions[9] = ToDistribution(30.88, 26.84, 25.32, 222.07, 118.65, 182.41);
            distributions[10] = ToDistribution(44.97, 85.96, 131.95, 651.32, 840.52, 963.67);
            distributions[11] = ToDistribution(236.02, 236.27, 230.70, 225.03, 117.29, 331.95);
            distributions[12] = ToDistribution(207.86, 191.20, 164.12, 494.04, 237.69, 533.52);
            distributions[13] = ToDistribution(99.83, 148.11, 188.17, 955.88, 654.95, 916.70);
            distributions[14] = ToDistribution(135.06, 131.92, 123.10, 350.35, 130.30, 388.43);
            distributions[15] = ToDistribution(135.96, 103.89, 66.88, 806.44, 642.20, 350.36);

            var proportions = new[] 
            {   
                0.0637, 0.0516, 0.0864, 0.0636, 0.0747, 0.0365, 0.0349, 0.0649,
                0.0656, 0.1189, 0.0362, 0.0849, 0.0368, 0.0389, 0.0943, 0.0477
            };

            return new Mixture<NormalDistribution>(proportions, distributions);
        }
    }
}
