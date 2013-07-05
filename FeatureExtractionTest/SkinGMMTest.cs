using System.Linq;
using FeatureExtraction;
using FeatureExtraction.Hand;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class SkinGMMTest
    {
        [TestMethod]
        public void TestModelsAreConsistent()
        {
            // check that probability is between 1 and 0 for all possible values of RGB
            foreach (var r in Enumerable.Range(0,25))
            {
                foreach (var g in Enumerable.Range(0,25))
                {
                    foreach (var b in Enumerable.Range(0, 25))
                    {
                        AssertColourProbabilityIsConsistent(r*10,g*10,b*10);
                    }
                }
            }
        }

        private static void AssertColourProbabilityIsConsistent(int r, int g, int b)
        {
            var p = SkinGMM.Get().Evaluate(new[] {r/255.0, g/255.0, b/255.0});
            Assert.IsTrue(p >= 0 && p <= 1);
        }
    }
}
