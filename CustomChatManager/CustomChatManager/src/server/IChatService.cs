namespace CustomChatManager.Server
{
	public interface IChatService
	{
		void processChatEvent(ChatMessageEvent e);
	}
}
