using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Engine.FeatureExtraction;

namespace Engine
{
    /// <summary>
    /// A Hidden Markov Model
    /// </summary>
    [Serializable]
    public class HMM
    {
        // Parameters for HMM learning and evaluation.
        private const double MinTolerance = 0.01;
        private const int MaxIterations = 100;
        private const double RegularisationFactor = 0.1;
        private const int HMMStateCount = 8;
        // ------------------------------------

        public ContinuousHiddenMarkovModel CHMM { get; private set; }

        public HMM(ContinuousHiddenMarkovModel signHMM)
        {
            CHMM = signHMM;
        }

        /// <summary>
        /// Create a default HMM, which has uniform output & transition distributions.
        /// Note: The structure of this HMM will be forward (aka left-right).
        /// </summary>
        /// <param name="dimensions">The dimension of the feature space</param>
        /// <returns>A default HMM</returns>
        public static HMM CreateDefault(int dimensions)
        {
            // Create a forward CHMM with uniform initial probabilities:
            return new HMM(new ContinuousHiddenMarkovModel(
                new Forward(HMMStateCount), 
                new NormalDistribution(dimensions)));
        }

        /// <summary>
        /// Train an Hidden Markov Model using the sequences
        /// 
        /// A subset of the features can be selected by the selectFeatures function.
        /// </summary>
        /// <param name="sequences">The set of training examples</param>
        /// <param name="selectFeatures">
        ///     A (function: Seq -> double[][]) which selects the relevant features from the sequence.
        /// </param>
        /// <returns>The trained Hidden Markov Model</returns>
        public static HMM CreateFromTrainingExamples(IList<Sequence> sequences, Func<Sequence, double[][]> selectFeatures)
        {
            // infer the dimensionality by looking at the first observation:
            int dimensions = selectFeatures(sequences[0])[0].Length;

            var hmm = CreateDefault(dimensions);
            hmm.Train(sequences, selectFeatures);
            return hmm;
        }

        /// <summary>
        /// Load a previously saved Hidden Markov Model
        /// </summary>
        /// <param name="path">The path of the file in which the HMM is saved</param>
        /// <returns>A Hidden Markov Model</returns>
        public static HMM CreateFromFile(string path)
        {
            // Serialise using the Binary Formatter.
            ContinuousHiddenMarkovModel hmm;
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                hmm = (ContinuousHiddenMarkovModel)(new BinaryFormatter()).Deserialize(file);
            }
            return new HMM(hmm);
        }

        /// <summary>
        /// Save the parameters and structure of the Hidden Markov Model into a file, 
        /// in binary format.
        /// </summary>
        /// <param name="path">The path of file in which to save the HMM</param>
        public void SaveToFile(string path)
        {
            // Deserialise using the Binary Formatter.
            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                (new BinaryFormatter()).Serialize(file, CHMM);
            }
        }

        /// <summary>
        /// Train this current Hidden Markov Model using the provided sequences.
        /// 
        /// A subset of the features can be selected by the selectFeatures function.
        /// </summary>
        /// <param name="sequences">The set of training examples</param>
        /// <param name="selectFeatures">
        ///     A (function: Seq -> double[][]) which selects the relevant features from the sequence.
        /// </param>
        public void Train(IList<Sequence> sequences, Func<Sequence, double[][]> selectFeatures)
        {
            var learner = new ContinuousBaumWelchLearning(CHMM)
            {
                Tolerance = MinTolerance,
                Iterations = MaxIterations,

                // This is necessary to prevent overfitting:
                FittingOptions = new NormalOptions {Regularization = RegularisationFactor}
            };

            learner.Run(sequences.Select(selectFeatures).ToArray()); 
        }

        private int Evaluate(double[][] sequence)
        {
            double result = CHMM.Evaluate(sequence);
            return (int)Math.Max(0,100 + Math.Log10(result)); // scale the result
        }

        /// <summary>
        /// Calculate the probability that the given HMM generated the seen observations.
        /// </summary>
        /// <param name="sequence">A sequence of observations</param>
        /// <param name="selectFeatures">A function which extracts the features of each frame</param>
        /// <returns>The Probability that the hmm generated the observations</returns>
        public int Evaluate(Sequence sequence, Func<Sequence, double[][]> selectFeatures)
        {
            return Evaluate(selectFeatures(sequence));
        }
    }
}
