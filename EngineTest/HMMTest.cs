using System.IO;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Engine;
using Engine.FeatureExtraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class HMMTest
    {
        private static double Precision = 0.01;

        [TestMethod]
        public void TestDeserialisationUndoesSerialisation()
        {
            const int states = 3;
            var origHMM = new ContinuousHiddenMarkovModel(states, new NormalDistribution(4));

            // change the model slightly.
            // i.e. make state 1 twice as likely, make state 0 not a starting state.
            origHMM.Probabilities[1] += origHMM.Probabilities[0];
            origHMM.Probabilities[0] = 0; 

            // save then load the model
            string path = Path.GetTempFileName();
            (new HMM(origHMM)).SaveToFile(path);

            var newHMM = HMM.CreateFromFile(path).CHMM;
            File.Delete(path);
            AssertHMMAreEqual(origHMM, newHMM);
            
        }

        public static void AssertHMMAreEqual(ContinuousHiddenMarkovModel expected, 
            ContinuousHiddenMarkovModel actual)
        {
            // ensure all parameters are still the same
            //  including Dimension, number of states,
            //  starting-state probabilities and transition matrix.
            Assert.AreEqual(expected.Dimension, actual.Dimension);
            Assert.AreEqual(expected.States, actual.States);
            AssertVectorsAreEqual(expected.Probabilities, actual.Probabilities);
            AssertMatricesAreEqual(expected.Transitions, actual.Transitions, expected.States);
        }

        private static void AssertVectorsAreEqual(double[] expected, double[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for(int i=0;i<expected.Length;i++)
                Assert.AreEqual(expected[i], actual[i], Precision, "Index: " + i);  
        }

        // assume square matrix
        private static void AssertMatricesAreEqual(double[,] expected, double[,] actual, int order)
        {
            for(int i=0;i<order;i++)
                for(int j=0;j<order;j++)
                    Assert.AreEqual(expected[i,j], actual[i,j], Precision, "Index: [{0},{1}]", i, j);
        }
    }
}
