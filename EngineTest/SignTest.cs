using System.IO;
using Engine;
using Engine.FeatureExtraction;
using Engine.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class SignTest
    {
        private HMM handHMM;
        private HMM skelHMM;
        private const string SignName = "Table";
        private const HandShapes HandShape = HandShapes.BladeSide;
        private const string ImageFile = @"C:\img\imgLoc.img";
        private const string VideoFile = @"C:\vid\vidLoc.vid";

        [TestInitialize]
        public void Setup()
        {
            // perform a modification: modify transition probabilities
            handHMM = HMM.CreateDefault(4);
            handHMM.CHMM.Transitions[0, 0] += handHMM.CHMM.Transitions[1, 1];
            handHMM.CHMM.Transitions[1, 1] = 0;

            // perform a modification: modify initial state probabilities
            skelHMM = HMM.CreateDefault(8);
            skelHMM.CHMM.Probabilities[0] += skelHMM.CHMM.Probabilities[1];
            skelHMM.CHMM.Probabilities[1] = 0;
        }

        [TestMethod]
        public void TestSavingThenLoadingResultsInNoChanges()
        {
            var engine = new EngineModel(skelHMM, handHMM);
            var orig = new Sign(SignName, HandShape, ImageFile, VideoFile, engine);
            var saveLocation = Sign.ToFile(orig);

            var loaded = Sign.FromFile(saveLocation);
            TestSignResourcesAreValid(loaded);
            Directory.GetCurrentDirectory();
            TestSignsLessResourcesAreEqual(orig, loaded);
        }

        private static void TestSignResourcesAreValid(Sign actual)
        {
            Assert.IsTrue(File.Exists(actual.ImgLocation) || Sign.InvalidImageFile.Equals(actual.ImgLocation),
                "image ({0}) not found", actual.ImgLocation);

            Assert.IsTrue(File.Exists(actual.VidLocation) || Sign.InvalidVideoFile.Equals(actual.VidLocation), 
                "video ({0}) not found", actual.VidLocation);
        }

        private static void TestSignsLessResourcesAreEqual(Sign expected, Sign actual)
        {
            // ignore resources, as they will be validated
            Assert.AreEqual(expected.SignClass, actual.SignClass);
            Assert.AreEqual(expected.SignName, actual.SignName);
            HMMTest.AssertHMMAreEqual(expected.SignModel.HandHMM.CHMM, actual.SignModel.HandHMM.CHMM);
            HMMTest.AssertHMMAreEqual(expected.SignModel.SkelHMM.CHMM, actual.SignModel.SkelHMM.CHMM);
        }
    }
}
