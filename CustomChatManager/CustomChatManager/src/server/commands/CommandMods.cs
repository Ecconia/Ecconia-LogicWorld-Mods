using System;
using System.Collections;
using System.Reflection;
using System.Text;
using LogicAPI;
using LogicAPI.Modding;
using LogicWorld.Server;
using LogicWorld.Server.Managers;

namespace CustomChatManager.Server.Commands
{
	public class CommandMods : ICommand
	{
		public string name => "Mods";
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
			mods = (IDictionary) field.GetValue(modManager);
		}

		public void execute(CommandSender sender, string arguments)
		{
			//There is always at least one mod installed, this one.
			StringBuilder sb = new StringBuilder();
			sb.Append("There are ")
			  .Append(ChatColors.highlight)
			  .Append(mods.Count)
			  .Append(ChatColors.close)
			  .Append(" mods installed:");
			foreach(object key in mods.Keys)
			{
				MetaMod mod = key as MetaMod;
				ModManifest manifest = mod.Manifest;
				//TBI: I bet some high-level prankster mod author gonna try to leave version or author away in future. I wonder if this will break then...
				sb.Append("\n - ")
				  .Append(ChatColors.highlight)
				  .Append(manifest.Name)
				  .Append(ChatColors.close)
				  .Append(" : ")
				  .Append(ChatColors.background)
				  .Append(manifest.Version)
				  .Append(ChatColors.close)
				  .Append(" by ")
				  .Append(ChatColors.background)
				  .Append(manifest.Author)
				  .Append(ChatColors.close);
			}
			sender.sendMessage(sb.ToString());
		}
	}
}
