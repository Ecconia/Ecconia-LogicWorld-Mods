namespace CustomChatManager.Server
{
	public interface ICommand
	{
		string name { get; }

		string shortDescription { get; }

		void execute(CommandSender sender, string arguments);
	}
}
