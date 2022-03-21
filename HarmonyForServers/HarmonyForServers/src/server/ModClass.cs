using AssemblyLoader.Shared;
using LogicAPI;
using LogicAPI.Server;
using ServerOnlyMods.Server;

namespace HarmonyForServers.Server
{
	public class ModClass : ServerMod, IServerSideOnlyMod
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
