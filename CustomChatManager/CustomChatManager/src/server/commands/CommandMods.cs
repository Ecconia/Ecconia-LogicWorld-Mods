using System.Collections;
using System.Text;
using EccsLogicWorldAPI.Shared.AccessHelper;
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
			var modManager = Program.Get<IModManager>();
			var field = Fields.getPrivate(typeof(ModManager), "ModScripts");
			var value = Fields.getNonNull(field, modManager);
			mods = Types.checkType<IDictionary>(value);
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
