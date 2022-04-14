using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LogicAPI;
using LogicAPI.Modding;
using LogicWorld.Server;
using LogicWorld.Server.Managers;

namespace CustomChatManager.Server.Commands
{
	public class CommandMods : ICommand
	{
		public string name => "mods";
		public string shortDescription => "Lists all installed server mods.";

		private readonly IDictionary mods;
		
		public CommandMods()
		{
			object modManager = Program.Get<IModManager>();
			var field = typeof(ModManager).GetField("ModScripts", BindingFlags.NonPublic | BindingFlags.Instance);
			if(field == null)
			{
				throw new Exception("Developer fix your mod, the ModScripts field does not exist.");
			}
			IDictionary dict = (IDictionary) field.GetValue(modManager);
			mods = dict;
		}
		
		public void execute(CommandSender sender, string arguments)
		{
			//There is always at least one mod installed, this one.
			sender.sendMessage("There are " + mods.Count + " mods installed:");
			foreach(object key in mods.Keys)
			{
				MetaMod mod = key as MetaMod;
				ModManifest manifest = mod.Manifest;
				//TBI: I bet some high-level prankster mod author gonna try to leave version or author away in future. I wonder if this will break then...
				sender.sendMessage(" - " + manifest.Name + " : " + manifest.Version + " by " + manifest.Author);
			}
		}
	}
}
