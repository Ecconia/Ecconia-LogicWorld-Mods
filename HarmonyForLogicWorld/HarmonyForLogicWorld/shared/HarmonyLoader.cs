using AssemblyLoader.Shared;
using LogicAPI;
using LogicLog;

namespace HarmonyForLogicWorld.Shared
{
	public class HarmonyLoader
	{
		public static void loadHarmony(ILogicLogger logger, IModFiles files, bool isClient)
		{
			foreach(ModFile modFile in files.EnumerateFiles())
			{
				if(
					".dll".Equals(modFile.Extension)
					&& (
						modFile.Path.StartsWith("dll/shared/")
						|| (isClient && modFile.Path.StartsWith("dll/client/"))
						|| (!isClient && modFile.Path.StartsWith("dll/server/"))
					)
				)
				{
					logger.Info("Loading '" + modFile.Path + "'.");
					DLLLoader.loadAssemblyFromModFile(modFile);
				}
			}
		}
	}
}
