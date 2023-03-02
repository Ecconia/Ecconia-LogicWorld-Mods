using System;
using System.Linq;
using AssemblyLoader.Shared;
using LogicAPI.Client;

namespace AssemblyLoader.client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			Logger.Info("AssemblyLoader Initialize...");

			foreach(var file in ModHelper.GetDependentModFiles(Files, Manifest.ID).Where(e => e.Path.Contains("client") || e.Path.Contains("shared")))
			{
				try
				{
					DLLLoader.loadAssemblyFromModFile(file);
					Logger.Info($"Load Assembly {file.FileName}");
				}
				catch(Exception e)
				{
					Logger.Error(e.ToString());
				}
			}
		}
	}
}
