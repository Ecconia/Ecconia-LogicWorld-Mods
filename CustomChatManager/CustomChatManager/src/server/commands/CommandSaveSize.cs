using System;
using JimmysUnityUtilities;
using LogicWorld.Server;
using LogicWorld.Server.Saving;

namespace CustomChatManager.Server.Commands
{
	public class CommandSaveSize : ICommand
	{
		public string name => "savesize";
		public string shortDescription => "Prints the save file size.";

		private readonly ISaveManager saveManager;

		public CommandSaveSize()
		{
			saveManager = Program.Get<ISaveManager>();
			if(saveManager == null)
			{
				throw new Exception("Could not get ISaveManager. /savesize will break");
			}
		}
		
		public void execute(CommandSender sender, string arguments)
		{
			sender.sendMessage("Current file size is: " + FileUtilities.PrettyDirectorySize(saveManager.ActiveSaveDirectory));
		}
	}
}
