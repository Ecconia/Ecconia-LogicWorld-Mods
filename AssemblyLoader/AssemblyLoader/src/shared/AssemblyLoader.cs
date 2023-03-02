using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using LogicAPI;
using LogicLog;
using LogicWorld.SharedCode.Modding;

namespace AssemblyLoader.Shared
{
	public static class AssemblyLoader
	{
		public static ILogicLogger logger;

		private static string cacheFolder;

		public static void loadAssemblyFromModFile(ModFile modFile)
		{
			logger.Debug("Loading mod file '" + modFile.Path + "' in mod-FS '" + modFile.FileSystem.Path + "'.");
			if(modFile.FileSystem is FolderModFiles)
			{
				var filePath = Path.Combine(modFile.FileSystem.Path, modFile.Path);
				loadAssembly(filePath);
				return;
			}
			//File system type is archive or something else, play safe and first extract the DLL into a cache folder:

			if(cacheFolder == null)
			{
				var temporaryDictionary = Path.GetTempPath();
				if(!Directory.Exists(temporaryDictionary))
				{
					throw new Exception("Could not setup AssemblyLoader, because temporary directory does not exist.");
				}
				cacheFolder = Path.Combine(temporaryDictionary, "LogicWorld-AssemblyLoader");
				Directory.CreateDirectory(cacheFolder);
			}

			//Create the cached-file-path:
			var fileHash = createHash(modFile);
			var cachedFilePath = Path.Combine(cacheFolder, string.Concat(modFile.FileName, "-", fileHash, modFile.Extension));

			//Always overwrite the file:
			using(Stream outputStream = File.Create(cachedFilePath))
			{
				using(Stream inputStream = modFile.OpenRead())
				{
					inputStream.CopyTo(outputStream);
				}
			}

			//Finally, load the assembly:
			loadAssembly(cachedFilePath);
		}

		private static void loadAssembly(string filePath)
		{
			try
			{
				Assembly.LoadFrom(filePath);
			}
			catch(Exception e)
			{
				logger.Error("Failed to load file at '" + filePath + "', because an " + e.GetType().Name + " got caught with message: " + e.Message);
			}
		}

		private static string createHash(ModFile modFile)
		{
			SHA256 sha256 = SHA256.Create();
			using(Stream stream = modFile.OpenRead())
			{
				return bytesToString(sha256.ComputeHash(stream));
			}
		}

		private static string bytesToString(byte[] bytes)
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
