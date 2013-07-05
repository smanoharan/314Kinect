using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class BaumWelchAlgorithmTest
    {
        private const double MinTolerance = 0.01;
        private const int MaxIterations = 100;
        private const double RegularisationFactor = 0.1;
        
        const int States = 3;
        const int Symbols = 2;
        const int YesUmbrella = 0;
        const int NoUmbrella = 1;


        [TestMethod]
        public void TestErrorRateDecreasesAfterEachIteration()
        {
            var hmm = new ContinuousHiddenMarkovModel(States, Symbols);
            var testSequence = new double[] {YesUmbrella, YesUmbrella, NoUmbrella};
            var initialError = 1 - hmm.Evaluate(testSequence);

            var trainedHmm = TrainHelper(new[] {testSequence});
            var errorAfterTraining = 1 - trainedHmm.Evaluate(testSequence);
            Assert.IsTrue(errorAfterTraining <= initialError);
        }


        private static ContinuousHiddenMarkovModel TrainHelper(double[][] sequences)
        {
            var hmm = new ContinuousHiddenMarkovModel(States, Symbols);
            var learner = new ContinuousBaumWelchLearning(hmm)
            {
                Tolerance = MinTolerance,
                Iterations = MaxIterations,
                FittingOptions = new NormalOptions()
                {
                    Regularization = RegularisationFactor
                }
            };

            learner.Run(sequences);
            return hmm;
        }
    }
}
