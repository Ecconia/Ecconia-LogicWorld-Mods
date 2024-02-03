namespace CustomChatManager.Server.Commands
{
	public interface ICommand
	{
		string name { get; }
		
		string shortDescription { get; }
		
		void execute(CommandSender sender, string arguments);
	}
}
