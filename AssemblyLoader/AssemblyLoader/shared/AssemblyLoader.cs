using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using LogicAPI;

namespace AssemblyLoader.Shared
{
	public static class DLLLoader
	{
		private static string cacheFolder;

		public static void setup()
		{
			var temporaryDictionary = Path.GetTempPath();
			if(!Directory.Exists(temporaryDictionary))
			{
				throw new Exception("Could not setup AssemblyLoader, because temporary directory does not exist.");
			}
			var cacheFolder = temporaryDictionary + "/LWAssemblyLoader/";
			Directory.CreateDirectory(cacheFolder);
			DLLLoader.cacheFolder = cacheFolder;
		}

		public static void loadAssemblyFromModFile(ModFile modFile)
		{
			if(cacheFolder == null)
			{
				setup();
			}

			//Create the cached-file-path:
			var fileHash = createHash(modFile);
			var cachedFilePath = string.Concat(cacheFolder, modFile.FileName, "-", fileHash, modFile.Extension);

			//Always overwrite the file:
			using(Stream outputStream = File.Create(cachedFilePath))
			{
				using(Stream inputStream = modFile.OpenRead())
				{
					inputStream.CopyTo(outputStream);
				}
			}

			//Load the assembly:
			Assembly.LoadFrom(cachedFilePath);
		}

		private static string createHash(ModFile modFile)
		{
			SHA256 sha256 = SHA256.Create();
			using(Stream stream = modFile.OpenRead())
			{
				return bytesToString(sha256.ComputeHash(stream));
			}
		}

		public static string bytesToString(byte[] bytes)
		{
			StringBuilder builder = new StringBuilder(bytes.Length * 2);
			foreach(byte b in bytes)
			{
				builder.Append(b.ToString("x2"));
			}
			return builder.ToString();
		}
	}
}
