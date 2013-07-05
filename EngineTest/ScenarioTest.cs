using System.IO;
using System.Text;
using System.Collections.Generic;
using Engine.Story;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest
{
    [TestClass]
    public class ScenarioTest
    {
        private LinkedList<Scenario> scenarioList;
        private const int ExpectedCount = 4;
        private static readonly Encoding StrEncoder = new UTF8Encoding();
        
        [TestInitialize]
        public void Setup()
        {
            const string scenarioText =
                "I know a little ______\n" +
                "know little ______\n" +
                "turtle-01-bg.png\n" +
                "turtle\n" +
                "His ____ is tiny Tim\n" +
                "____ Tim\n" +
                "turtle-02-bg.png\n" +
                "name\n" +
                "I put him in the ____ ___\n" +
                "put ____ ___\n" +
                "turtle-03-bg.png\n" +
                "bath-tub\n" +
                "To see if he could ____\n" +
                "see ____\n" +
                "turtle-04-bg.png\n" +
                "swim\n";

            scenarioList = CreateScenarioListFromString(scenarioText);
        }

        private static LinkedList<Scenario> CreateScenarioListFromString(string data)
        {
            return Scenario.FromFile(new MemoryStream(StrEncoder.GetBytes(data)));
        }

        [TestMethod]
        public void TestScenarioCountIsAsExpected()
        {
            Assert.AreEqual(ExpectedCount, scenarioList.Count);
        }

        [TestMethod]
        public void TestFirstScenarioAttributes()
        {
            var firstScenario = scenarioList.First.Value;
            Assert.AreEqual("I know a little ______", firstScenario.EnglishLine);
            Assert.AreEqual("turtle", firstScenario.SignName);
            Assert.AreEqual("know little ______", firstScenario.SignLine);
            Assert.AreEqual("turtle-01-bg.png", firstScenario.BgResourcePath);
        }

        [TestMethod]
        public void TestLastScenarioAttributes()
        {
            var lastScenario = scenarioList.Last.Value;
            Assert.AreEqual("To see if he could ____", lastScenario.EnglishLine);
            Assert.AreEqual("swim", lastScenario.SignName);
            Assert.AreEqual("see ____", lastScenario.SignLine);
            Assert.AreEqual("turtle-04-bg.png", lastScenario.BgResourcePath);
        }

        [TestMethod]
        public void TestInvalidScenarioDataThrowsException()
        {
            const string emptyScenario = "";
            TestDataIsInvalid(emptyScenario, "emptyScenario");

            const string missingNewLineScenario = 
                "line(1)\nline(2)\nline3\n";
            TestDataIsInvalid(missingNewLineScenario, "missingNewLineScenario");

            const string missingFinalNewLineScenario =
                "line1(1)\nline(1)2\nline13\nline14\n" +
                "line2(1)\nline(2)2\nline23\n";
            TestDataIsInvalid(missingFinalNewLineScenario, "missingFinalNewLineScenario");

            const string longInvalidScenario =
                "line(1)1\nline(1)2\nline13\nline14\n" +
                "line2(1)\nline2(2)\nline23\nline24\n" +
                "line3(1)\nline3(2)\nline33\nline34\n" +
                "line4(1)\nline4(2)";
            TestDataIsInvalid(longInvalidScenario, "longInvalidScenario");

            const string missingEnglishLine =
                "\nline(1)2\nline13\nline44\n";
            TestDataIsInvalid(missingEnglishLine, "missingEnglishLine"); 
            
            const string missingSignLine =
                 "line11\n\nline13\nline(1)2\n";
            TestDataIsInvalid(missingSignLine, "missingSignLine");

            const string missingResource =
                 "line11\nline(1)2\n\nline13\n";
            TestDataIsInvalid(missingResource, "missingResource");

            const string missingSignWord =
                "line11\nline(1)2\nline13\n\n";
            TestDataIsInvalid(missingSignWord, "missingSignWord");
        }

        private static void TestDataIsInvalid(string dataString, string caseName)
        {
            try
            {
                CreateScenarioListFromString(dataString); 
                Assert.Fail(caseName + " did not throw an exception");
            }
            catch (InvalidDataException)
            {
                Assert.IsTrue(true); // pass the test
            }
        }
    }
}
