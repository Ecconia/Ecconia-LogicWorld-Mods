using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using LogicAPI;
using LogicLog;

namespace AssemblyLoader.Shared
{
	public static class DLLLoader
	{
		private static string cacheFolder;

        private static ILogicLogger logger = LogicLogger.For("DllLoader");

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

            PrintFileInfo(modFile);

			//Copy Assembly to Cache folder
            CreateCachedCopy(modFile);


			var symName = modFile.Path.Replace(".dll", ".pdb");

            if (modFile.FileSystem.Exists(symName))
			{
				ModFile symFile = modFile.FileSystem.GetFile(symName);
				PrintFileInfo(symFile);
				CreateCachedCopy(symFile);
				//Read Bytes for Assembly and its Symbols    
				var cachedAssemblyBytes = File.ReadAllBytes(GetCachedFilePath(modFile));
				var cachedSymbolsBytes = File.ReadAllBytes(GetCachedFilePath(symFile));
				var aName = AssemblyName.GetAssemblyName(GetCachedFilePath(modFile));
				Assembly.Load(aName);
				//var ass = Assembly.Load(cachedAssemblyBytes, cachedSymbolsBytes);
				return;
			}

			//Load the assembly:
			Assembly.LoadFrom(GetCachedFilePath(modFile));

			void CreateCachedCopy(ModFile file)
            {
                var cachepath = GetCachedFilePath(file);
                if (File.Exists(cachepath)) return;
                File.Copy(GetFileFullPath(file), GetCachedFilePath(file));
            }

            string GetCachedFilePath(ModFile file) => string.Concat(cacheFolder, file.FileName.Replace(file.Extension,""), "-", createHash(file), file.Extension);

            void PrintFileInfo(ModFile file)
            {
				logger.Info("Folder: " + file.FileSystem.Path);
                logger.Info("Name: " + file.FileName);
                logger.Info("Ext: " + file.Extension);
                logger.Info("Path: " + file.Path);
            }

            string GetFileFullPath(ModFile file) => Path.Combine(file.FileSystem.Path, file.Path.Replace('/',Path.DirectorySeparatorChar));

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
