using System.Collections;
using System.IO;

namespace FeatureExtraction.Hand
{
    public static class PPM
    {
        public static void createFromBitArr(string name, BitArray ba, int width, int height)
        {
            string filename = name + ".ppm";
            StreamWriter sw = new StreamWriter(filename);
            sw.Write("P3\n");
            sw.Write("# CREATOR: GIMP PNM Filter Version 1.1\n");
            sw.Write(width + " " + height + "\n");
            sw.Write("255\n");

            //Image needs white around the outside
            for (int i = 0; i < width; i++) ba[i] = false; //top of the image
            for (int i = width * (height - 1); i < width * height; i++) ba[i] = false; //bottom of the image
            for (int i = 0; i < width * height; i += height) ba[i] = false; //left of the image
            for (int i = height - 1; i < width * height; i += height) ba[i] = false; //right of the image

            //Write out image to file
            for (int i = 0; i < ba.Count; i++)
            {
                sw.Write(ba[i] ? "0\n" : "255\n");
                sw.Write(ba[i] ? "0\n" : "255\n");
                sw.Write(ba[i] ? "0\n" : "255\n");
            }

            sw.Close();
        }
    }
}
