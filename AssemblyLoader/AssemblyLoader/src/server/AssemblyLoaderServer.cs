using System.Collections.Generic;
using AssemblyLoader.Shared;
using LogicAPI;
using LogicAPI.Server;
using LogicWorld.SharedCode.Modding;

namespace AssemblyLoader.Server
{
	public class AssemblyLoaderServer : ServerMod
	{
		//Expose this field, for other mods to be able to use it:
		public static IList<MetaMod> installedMods;

		protected override void Initialize()
		{
			//Set the logger, to be used when loading assemblies:
			AssemblyLoader.Shared.AssemblyLoader.logger = Logger;
			
			//The following step is rather expensive, as it will find mods again, which LW-Server already did.
			// But LW-Server did not save the result anywhere and is straight up processing it.
			// The safest least hacky way of obtaining the list, is to simply scan for mods again.
			//This will print the warning about invalids mods again though.
			//In return the available mods will be available via this mod now.
			installedMods = ModLoader.FindMods();
			//Automatically load assemblies for mods that depend on this mod:
			AutoLoader.autoLoad(installedMods, Manifest.ID, ModSide.Server);
		}
	}
}
