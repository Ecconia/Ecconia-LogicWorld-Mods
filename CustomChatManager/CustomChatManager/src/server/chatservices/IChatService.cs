namespace CustomChatManager.Server.ChatServices
{
	public interface IChatService
	{
		void processChatEvent(ChatMessageEvent e);
	}
}
