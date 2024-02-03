using System.Collections.Generic;
using System.Reflection;
using AssemblyLoader.Shared;
using LogicAPI;
using LogicAPI.Server;
using LogicLog;
using LogicWorld.Server;
using LogicWorld.Server.Managers;

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
			
			//Access the list of installed but unloaded mods.
			// They are hidden in a private field. Which I added to LW just to access it here :P
			var installedMods = getMods(Logger);
			if(installedMods != null)
			{
				//Automatically load assemblies for mods that depend on this mod:
				AutoLoader.autoLoad(installedMods, Manifest.ID, ModSide.Server);
			}
		}
		
		private static IList<MetaMod> getMods(ILogicLogger logger)
		{
			var modLoader = Program.Get<IModManager>();
			if(modLoader == null)
			{
				logger.Error("Service of type 'IModManager' does not exist. Not able to automatically load assemblies for other mods.");
				return null;
			}
			if(modLoader.GetType() != typeof(ModManager))
			{
				logger.Error("Service of type 'IModManager' is not a 'ModManager' class. Not able to automatically load assemblies for other mods.");
				return null;
			}
			
			var fieldMetaMods = modLoader.GetType().GetField("MetaMods", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldMetaMods == null)
			{
				logger.Error("Could not find field 'MetaMods' in class 'ModManager'. Not able to automatically load assemblies for other mods.");
				return null;
			}
			var value = fieldMetaMods.GetValue(modLoader);
			if(value is IList<MetaMod> metaMods)
			{
				return metaMods;
			}
			logger.Error("Field 'MetaMods' in class 'ModManager' is 'null' or wrong type. Not able to automatically load assemblies for other mods.");
			return null;
		}
	}
}
