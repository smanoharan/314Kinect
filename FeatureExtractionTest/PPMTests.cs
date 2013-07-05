using System.Collections;
using System.IO;
using FeatureExtraction;
using FeatureExtraction.Hand;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeatureExtractionTest
{
    [TestClass]
    public class PPMTests
    {
        [TestMethod]
        public void CreateFromBitArr255s()
        {
            int h = 3;
            int w = 3;
            string filename = "pmmtest";
            BitArray ba = new BitArray(w*h);
            PPM.createFromBitArr(filename, ba, 3, 3);

            StreamReader sr = new StreamReader(filename + ".ppm");
            //PPM Header
            Assert.AreEqual(sr.ReadLine(),"P3");
            Assert.AreEqual(sr.ReadLine(), "# CREATOR: GIMP PNM Filter Version 1.1");
            Assert.AreEqual(sr.ReadLine(), w + " " + h);
            Assert.AreEqual(sr.ReadLine(), "255");

            //Test Data
            for (int i = 0; i < (w * h); i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255");
            }
            sr.Close();
            File.Delete(filename + ".ppm");
        }

        [TestMethod]
        public void CreateFromBitArr0s()
        {
            int h = 3;
            int w = 3;
            string filename = "pmmtest2";
            BitArray ba = new BitArray(w * h);
            for (int i = 0; i < (h*w); i++) ba[i] = true;
            PPM.createFromBitArr(filename, ba, 3, 3);
            StreamReader sr = new StreamReader(filename + ".ppm");
            
            //PPM Header
            Assert.AreEqual(sr.ReadLine(), "P3");
            Assert.AreEqual(sr.ReadLine(), "# CREATOR: GIMP PNM Filter Version 1.1");
            Assert.AreEqual(sr.ReadLine(), w + " " + h);
            Assert.AreEqual(sr.ReadLine(), "255");
            
            //Test Data
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //Since Outline gets cleared
            }
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(sr.ReadLine(), "0"); //Middle Point
            }
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //Since Outline gets cleared
            }
            sr.Close();
            File.Delete(filename + ".ppm");
        }

        [TestMethod]
        public void CreateFromBitArrMix()
        {
            int h = 3;
            int w = 3;
            string filename = "pmmtest3";
            BitArray ba = new BitArray(w * h);
            for (int i = 2; i < ba.Count; i++) ba[i] = true;
            PPM.createFromBitArr(filename, ba, 3, 3);
            StreamReader sr = new StreamReader(filename + ".ppm");

            //PPM Header
            Assert.AreEqual(sr.ReadLine(), "P3");
            Assert.AreEqual(sr.ReadLine(), "# CREATOR: GIMP PNM Filter Version 1.1");
            Assert.AreEqual(sr.ReadLine(), w + " " + h);
            Assert.AreEqual(sr.ReadLine(), "255");

            //Test Data
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //Since Outline gets cleared
            }
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(sr.ReadLine(), "0"); //Middle Point
            }
            for (int i = 0; i < 12; i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //Since Outline gets cleared
            }

            sr.Close();
            File.Delete(filename + ".ppm");
        }

        [TestMethod]
        public void CreateFromBitArrMixLarge()
        {
            int h = 10;
            int w = 10;
            string filename = "pmmtest4";
            BitArray ba = new BitArray(w * h);
            for (int i = 50; i < ba.Count; i++) ba[i] = true;
            PPM.createFromBitArr(filename, ba, w, h);
            StreamReader sr = new StreamReader(filename + ".ppm");

            //PPM Header
            Assert.AreEqual(sr.ReadLine(), "P3");
            Assert.AreEqual(sr.ReadLine(), "# CREATOR: GIMP PNM Filter Version 1.1");
            Assert.AreEqual(sr.ReadLine(), w + " " + h);
            Assert.AreEqual(sr.ReadLine(), "255");

            //Test Data
            for (int i = 0; i < (w*3); i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //Since Outline gets cleared
            }
            for (int l = 0; l < (4*3*w); l++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //first half is white
            }
            for (int l = 0; l < 4; l++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(sr.ReadLine(), "255"); //Middle Point
                }
                for (int i = 0; i < 24; i++)
                {
                    Assert.AreEqual(sr.ReadLine(), "0"); //Middle Point
                }
                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(sr.ReadLine(), "255"); //Middle Point
                }
            }
            for (int i = 0; i < (w * 3); i++)
            {
                Assert.AreEqual(sr.ReadLine(), "255"); //Since Outline gets cleared
            }

            sr.Close();
            File.Delete(filename + ".ppm");
        }
    }
}
