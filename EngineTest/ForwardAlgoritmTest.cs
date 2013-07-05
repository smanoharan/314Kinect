using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Models.Markov;
using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class ForwardAlgoritmTest
    {
        private HiddenMarkovModel hmm;
        private const double Precision = 0.00001;

        const int States = 3;
        const int Sunny = 0;
        const int Rainy = 1;
        const int Windy = 2;

        const int Symbols = 2;
        const int YesUmbrella = 0;
        const int NoUmbrella = 1;

        [TestInitialize]
        public void Setup()
        {
            // build up the simple weather model:
            //      states = { Sunny, Rainy, Windy } 
            //      observations = { yes, no }
            
            // Start state probabilties :
            var initial = new double[States];
            initial[Sunny] = 0.5;
            initial[Rainy] = 0.1;
            initial[Windy] = 0.4;

            // Transition Matrix :
            var transitions = new double[States, States];

            // if today is sunny:
            //      P(tomorrow is sunny) = 0.5
            //      P(tomorrow is windy) = 0.4
            //      P(tomorrow is rainy) = 0.1
            transitions[Sunny, Sunny] = 0.5;
            transitions[Sunny, Windy] = 0.4;
            transitions[Sunny, Rainy] = 0.1;

            // if today is windy:
            //      P(tomorrow is sunny) = 0.3
            //      P(tomorrow is windy) = 0.4
            //      P(tomorrow is rainy) = 0.3
            transitions[Windy, Sunny] = 0.3;
            transitions[Windy, Windy] = 0.4;
            transitions[Windy, Rainy] = 0.3;

            // if today is rainy:
            //      P(tomorrow is sunny) = 0.1
            //      P(tomorrow is windy) = 0.1
            //      P(tomorrow is rainy) = 0.8
            transitions[Rainy, Sunny] = 0.1;
            transitions[Rainy, Windy] = 0.1;
            transitions[Rainy, Rainy] = 0.8;

            // Emission probabilities:
            var emissions = new double[States, Symbols];

            // if sunny : P (umbrella-present) = 0.4
            // if windy : P (umbrella-present) = 0.1
            // if rainy : P (umbrella-present) = 0.8
            emissions[Sunny, YesUmbrella] = 0.4;
            emissions[Windy, YesUmbrella] = 0.1;
            emissions[Rainy, YesUmbrella] = 0.8;

            // probabilities of P(umbrella-absent) are just the complements:
            for (int i = 0; i < States; i++)
            {
                emissions[i, NoUmbrella] = 1 - emissions[i, YesUmbrella];
            }
            
            // create the hmm with the given parameters:
            hmm = new HiddenMarkovModel(transitions, emissions, initial);
        }

        [TestMethod]
        public void TestInitialProbabilitiesAreCorrect()
        {
            // Probability of no umbrella being seen:
            //      expected    = P(sunny)*P(no|sunny) + P(windy)*P(no|windy) + P(rainy)*P(no|rainy)
            //                  = 0.5*0.6 + 0.4*0.9 + 0.1*0.2
            //                  = 0.68
            Assert.AreEqual(0.68, Evaluate(hmm, new[] {NoUmbrella}), Precision);

            // Probability of umbrella being seen:
            //      expected    = P(sunny)*P(yes|sunny) + P(windy)*P(yes|windy) + P(rainy)*P(yes|rainy)
            //                  = 0.5*0.4 + 0.4*0.1 + 0.1*0.8 
            //                  = 0.32
            Assert.AreEqual(0.32, Evaluate(hmm, new[] {YesUmbrella}), Precision);
        }

        [TestMethod]
        public void TestMultipleObservationProbabilitiesAreCorrect()
        {
            // Probability of umbrella-umbrella sequence:
            //      expected = P(yes, yes) 
            //
            //  There are 3x3 = 9 possible state sequences:
            //      if sunny (P = 0.5*0.4 = 0.2), then
            //         sunny-sunny : P(yes, yes) = 0.2*0.5*0.4      = 0.0400
            //         sunny-windy : P(yes, yes) = 0.2*0.4*0.1      = 0.0080
            //         sunny-rainy : P(yes, yes) = 0.2*0.1*0.8      = 0.0160
            //
            //      if windy (P = 0.4*0.1 = 0.04), then
            //         windy-sunny : P(yes, yes) = 0.04*0.3*0.4     = 0.0048
            //         windy-windy : P(yes, yes) = 0.04*0.4*0.1     = 0.0016
            //         windy-rainy : P(yes, yes) = 0.04*0.3*0.8     = 0.0096
            //
            //      if rainy (P = 0.1*0.8 = 0.08), then
            //         rainy-sunny : P(yes, yes) = 0.08*0.1*0.4     = 0.0032
            //         rainy-windy : P(yes, yes) = 0.08*0.1*0.1     = 0.0008
            //         rainy-rainy : P(yes, yes) = 0.08*0.8*0.8     = 0.0512
            //                                                  
            //      SUM                                             = 0.064 + 0.016 + 0.0552
            //                                                      = 0.1352
            Assert.AreEqual(0.1352, Evaluate(hmm, new[] { YesUmbrella, YesUmbrella }), Precision);
        }

        [TestMethod]
        public void TestMultipleObservationProbabilitiesAreConsistent()
        {
            // Ensure the probability of all possible state sequences of a particular order sum to 1.
            // check all sequences of order 3 = 8 sequences

            double sumOfProbabilities = 0;
            for (int first = 0; first < Symbols; first++)
                for (int second = 0; second < Symbols; second++)
                    for (int third = 0; third < Symbols; third++)
                        sumOfProbabilities += Evaluate(hmm, new[] {first, second, third});

            Assert.AreEqual(1, sumOfProbabilities, Precision);
        }

        private static double Evaluate(HiddenMarkovModel hmm, int[] sequence)
        {
            return hmm.Evaluate(sequence);
        }
    }
}
