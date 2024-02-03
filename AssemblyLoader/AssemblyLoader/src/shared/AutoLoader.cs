using System.Collections.Generic;
using System.Linq;
using LogicAPI;

namespace AssemblyLoader.Shared
{
	public static class AutoLoader
	{
		public static void autoLoad(IEnumerable<MetaMod> metaMods, string assemblyLoaderModID, ModSide side)
		{
			foreach(var metaMod in metaMods)
			{
				if(!metaMod.Manifest.Dependencies.Contains(assemblyLoaderModID))
				{
					continue;
				}
				//Mod depends on AssemblyLoader, try to load assemblies:
				autoLoadAssembliesForMod(metaMod, side);
			}
		}

		private static void autoLoadAssembliesForMod(MetaMod mod, ModSide side)
		{
			foreach(var modFile in mod.Files.EnumerateFiles())
			{
				if(
					".dll".Equals(modFile.Extension)
					&& (
						modFile.Path.StartsWith("assemblies/shared/")
						|| (side == ModSide.Client && modFile.Path.StartsWith("assemblies/client/"))
						|| (side == ModSide.Server && modFile.Path.StartsWith("assemblies/server/"))
					)
				)
				{
					AssemblyLoaderShared.loadAssemblyFromModFile(modFile);
				}
			}
		}
	}
}
