using System.Reflection;
using AssemblyLoader.Shared;
using LogicAPI;
using LogicAPI.Client;
using LogicLog;
using LogicWorld.Modding;

namespace AssemblyLoader.Client
{
	public class AssemblyLoaderClient : ClientMod
	{
		protected override void Initialize()
		{
			//Set the logger, to be used when loading assemblies:
			AssemblyLoaderShared.logger = Logger;
			
			//Load all mods (active or not), so that they can be probed for assemblies to load:
			var metaMods = getMods(Logger);
			if(metaMods != null)
			{
				//Automatically load assemblies for mods that depend on this mod:
				AutoLoader.autoLoad(metaMods, Manifest.ID, ModSide.Client);
			}
		}
		
		private static MetaMod[] getMods(ILogicLogger logger)
		{
			var typeModLoader = typeof(Mods).Assembly.GetType("LogicWorld.Modding.Loading.ModLoader");
			if(typeModLoader == null)
			{
				logger.Error("Could not find class 'ModLoader' next to 'Mods'. Not able to automatically load assemblies for other mods.");
				return null;
			}
			var fieldMetaMods = typeModLoader.GetField("MetaMods", BindingFlags.NonPublic | BindingFlags.Static);
			if(fieldMetaMods == null)
			{
				logger.Error("Could not find field 'MetaMods' in class 'ModLoader'. Not able to automatically load assemblies for other mods.");
				return null;
			}
			var value = fieldMetaMods.GetValue(null);
			if(value is MetaMod[] metaMods)
			{
				return metaMods;
			}
			logger.Error("Field 'MetaMods' in class 'ModLoader' is 'null'. Not able to automatically load assemblies for other mods.");
			return null;
		}
	}
}
