using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Engine.Story
{
    /// <summary>
    /// Represents a single line of a nursery rhyme
    /// </summary>
    public class Scenario
    {
        public string SignName { get; private set; }
        public string EnglishLine { get; private set; }
        public string BgResourcePath { get; private set; }
        public string SignLine { get; private set; }
        public string PreviousLines { get; private set; }

        /// <summary>
        /// Load the list of sequences from the file
        /// 
        /// Throws an InvalidDataException if data format is not as expected.
        /// </summary>
        /// <param name="ins">The stream which contains the string</param>
        /// <returns>A list of sequences (representing a story)</returns>
        public static LinkedList<Scenario> FromFile(Stream ins)
        {
            var scenarioList = new LinkedList<Scenario>();
            using (var sReader = new StreamReader(ins))
            {
                StringBuilder prevLines = new StringBuilder();
                if (sReader.EndOfStream) throw new InvalidDataException("Story cannot be empty.");
                while (!sReader.EndOfStream)
                {
                    var scene = new Scenario(sReader, prevLines.ToString());
                    scenarioList.AddLast(scene);
                    prevLines.Append("\n" + Regex.Replace(scene.EnglishLine, "_+", scene.SignName.ToLowerInvariant()));
                }
            }
            return scenarioList;
        }

        private Scenario(StreamReader sReader, string prevLines)
        {
            try
            {
                // format: 
                //  english-grammer-line\n
                //  sign-grammer-line\n
                //  background-resource\n
                //  missing-word\n
            
                EnglishLine = sReader.ReadLine();
                SignLine = sReader.ReadLine();
                BgResourcePath = sReader.ReadLine();
                SignName = sReader.ReadLine();
                PreviousLines = prevLines;
                
                // validate the arguments:
                var args = (new[] {EnglishLine, SignLine, BgResourcePath, SignName});
                if (args.Any(String.IsNullOrEmpty)) 
                    throw new Exception("Invalid format.");

            }
            catch (Exception e)
            {
                throw new InvalidDataException("Invalid format.", e);
            }
        }
    }
}
