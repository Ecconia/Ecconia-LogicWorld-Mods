using AssemblyLoader.Shared;
using LogicAPI;
using LogicAPI.Server;

namespace HarmonyForServers.Server
{
	public class ModClass : ServerMod
	{
		protected override void Initialize()
		{
			foreach(ModFile modFile in Files.EnumerateFiles())
			{
				if(".dll".Equals(modFile.Extension))
				{
					Logger.Info("Loading '" + modFile.FileName + "'.");
					DLLLoader.loadAssemblyFromModFile(modFile);
				}
			}
		}
	}
}
