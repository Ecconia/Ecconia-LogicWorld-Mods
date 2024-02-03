using EccsLogicWorldAPI.Server;
using JimmysUnityUtilities;
using LogicWorld.Server.Saving;

namespace CustomChatManager.Server.Commands
{
	public class CommandSaveSize : ICommand
	{
		public string name => "SaveSize";
		public string shortDescription => "Prints the save file size.";
		
		private readonly ISaveManager saveManager;
		
		public CommandSaveSize()
		{
			saveManager = ServiceGetter.getService<ISaveManager>();
		}
		
		public void execute(CommandSender sender, string arguments)
		{
			sender.sendMessage("Current file size is: " + ChatColors.highlight + FileUtilities.PrettyDirectorySize(saveManager.ActiveSaveDirectory) + ChatColors.close);
		}
	}
}
