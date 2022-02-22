using System;
using System.IO;
using System.Reflection;

using LogicAPI;
using LogicAPI.Client;

using UnityEngine;

namespace DllUtil
{
	public class Loader: ClientMod
	{
		private static Loader instance = null;
		
		private string root = null;
		
		protected override void Initialize()
		{
			instance = this;
			Logger.Info("Initializing DllUtil...");
			root = string.Concat(Application.temporaryCachePath, "/dllcache/");
			Logger.Trace("Clearing cache directory...");
			try
			{
				Directory.Delete(root, true);
			}
			catch (Exception ignored)
			{
			}
			Directory.CreateDirectory(root);
		}
		
		internal Assembly LoadInternal(ModFile file)
		{
			try
			{
				Logger.Info(string.Concat("Loading ", file.FileName));
				var path = string.Concat(root, file.FileName, System.Guid.NewGuid().ToString(), file.Extension);
				Logger.Trace(string.Concat("Exporting ", file.FileName, " to ", path));
				var fileStream = File.Create(path);
				var modStream = file.OpenRead();
				modStream.CopyTo(fileStream);
				fileStream.Close();
				Logger.Trace(string.Concat("Successfully exported ", file.FileName, "!"));
				var assembly = Assembly.LoadFrom(path);
				Logger.Info(string.Concat("Successfully loaded ", file.FileName));
				return assembly;
			}
			catch (Exception e)
			{
				Logger.Fatal(string.Concat("Failed to load ", file.FileName, "!\n", e.StackTrace));
				return null;
			}
		} 
		
		public static Assembly Load(ModFile file)
		{
			return instance.LoadInternal(file);
		}
	}
}
