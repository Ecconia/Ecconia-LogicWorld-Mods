using AssemblyLoader.Shared;
using LogicAPI.Server;
using LogicWorld.SharedCode.Modding;

namespace AssemblyLoader.Server
{
	public class AssemblyLoaderServer : ServerMod
	{
		protected override void Initialize()
		{
			//Set the logger, to be used when loading assemblies:
			AssemblyLoaderShared.logger = Logger;
			
			//Since 0.91 the server does no longer load all DLLs it comes with.
			// Some mods however need some of these DLLs to compile, the compilation will fail.
			// As solution one can ensure that every delivered DLL is loaded.
			//Originally this issue occurred first for cheese's util mods.
			// Which CheeseLoader was written for.
			// This mod will have the same functionality, as both mods can coexist.
			// It is one less required dependency for users of Eccs mods.
			//Part of the reasons for the reimplementation is that currently CheeseLoader does not support ServerOnlyMods.
			ServerAssemblyLoader.loadAllServerAssemblies(Logger);
			
			//Automatically load assemblies for mods that depend on this mod:
			AutoLoader.autoLoad(ModRegistry.InstalledMods, Manifest.ID, ModSide.Server);
		}
	}
}
