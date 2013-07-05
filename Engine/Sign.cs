using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Engine
{
	/// <summary>
	/// Represents a single sign (along with the associated model trained from examples)
	/// </summary>
	[Serializable()]
	public class Sign
	{
		public const string InvalidImageFile = "../../Images/filePathInvalid.png";
		public const string InvalidVideoFile = "../../Images/invalidVideoPath.png";

		public string SignName {get; set;}
		public HandShapes SignClass { get; set; }
		public string ImgLocation { get; set; }
		public string VidLocation { get; set; }
	    public readonly EngineModel SignModel;

		private static Dictionary<string, string> signFilePath;
		private static Dictionary<HandShapes, LinkedList<string>> signsByHandShape;
		static Sign()
		{
			UpdateSignList();
		}

		/// <summary>
		/// Update the Sign List, by checking the signs on disk.
		/// </summary>
		public static void UpdateSignList()
		{
			signFilePath = new Dictionary<string, string>();
			signsByHandShape = new Dictionary<HandShapes, LinkedList<string>>();

			// find all the *.sign files in the current directory.
			foreach (var d in Directory.GetDirectories(Directory.GetCurrentDirectory()))
			{
				HandShapes handShape;
				
				// ignore non-hand-shape directories
				if (!Enum.TryParse(d.Split('\\').Last(), out handShape)) continue;

				LinkedList<string> signs = new LinkedList<string>();
				foreach (var file in Directory.GetFiles(d, "*.sign"))
				{
					string signName = file.Split('\\').Last().Split('.').First();
					signFilePath.Add(signName, file);
				}
				signsByHandShape.Add(handShape, signs);
			}
		}

        public Sign()
        {
            SignName = "";
            SignClass = HandShapes.NoClass;
            ImgLocation = "";
            VidLocation = "";
            SignModel = null;
        }

        public Sign(string signName, HandShapes signClass, string imgLocation, string vidLocation, EngineModel signModel)
        {
            SignName = signName;
            SignClass = signClass;
            ImgLocation = imgLocation;
            VidLocation = vidLocation;
            SignModel = signModel;
        }

		/// <summary>
		/// Save the file to file in a directory determined by its handshape.
		/// </summary>
		/// <param name="sign">The sign to save</param>
		/// <returns>The location of the file in which the sign has been saved</returns>
		public static string ToFile(Sign sign)
		{
			string outputDirectory = Directory.GetCurrentDirectory() + "\\" + sign.SignClass;
			if (!Directory.Exists(outputDirectory)) 
				Directory.CreateDirectory(outputDirectory);

			string outFile = outputDirectory + "\\" + sign.SignName + ".sign";
			using (var outFS = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				(new BinaryFormatter()).Serialize(outFS, sign);
			}

			return outFile;
		}

		/// <summary>
		/// Find a sign by name.
		/// If the sign is not found, null is returned.
		/// </summary>
		/// <param name="signName">The name of the sign to look for</param>
		/// <returns>The sign</returns>
		public static Sign FromName(string signName)
		{
			return signFilePath.ContainsKey(signName) ? FromFile(signFilePath[signName]) : null;
		}

		/// <summary>
		/// Load a sign from a file. 
		/// Also, perform validation to ensure that any non-existant resources are replaced
		///     with default resources.
		/// </summary>
		/// <param name="filePath">Where the Sign has been saved</param>
		/// <returns>The validated sign</returns>
		public static Sign FromFile(string filePath)
		{
			Sign sign;
			using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				sign = (Sign)((new BinaryFormatter()).Deserialize(file));
			}

			// ensure all resources exist (if not, replace with default resources) : TEMP res at the moment.
			if (!File.Exists(sign.ImgLocation)) sign.ImgLocation = InvalidImageFile;
			if (!File.Exists(sign.VidLocation)) sign.VidLocation = InvalidVideoFile;
			
			return sign;
		}
	}
}   