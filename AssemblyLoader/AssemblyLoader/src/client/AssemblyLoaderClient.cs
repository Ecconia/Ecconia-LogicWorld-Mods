using AssemblyLoader.Shared;
using LogicAPI.Client;
using LogicWorld.SharedCode.Modding;

namespace AssemblyLoader.Client
{
	public class AssemblyLoaderClient : ClientMod
	{
		protected override void Initialize()
		{
			//Set the logger, to be used when loading assemblies:
			AssemblyLoaderShared.logger = Logger;
			
			//Automatically load assemblies for mods that depend on this mod:
			AutoLoader.autoLoad(ModRegistry.InstalledMods, Manifest.ID, ModSide.Client);
		}
	}
}
