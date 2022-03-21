using LogicAPI.Data;
using LogicAPI.Networking;

namespace CustomChatManager.Server
{
	public class ChatMessageEvent
	{
		public Connection sender;
		public ChatMessageData originalMessage;

		public MessageEventResult result = MessageEventResult.Send;
		public ChatMessageData responseMessage;

		public ChatMessageEvent(ChatMessageData originalMessage, Connection sender)
		{
			this.sender = sender;
			this.originalMessage = originalMessage;

			responseMessage = new ChatMessageData
			{
				Color = originalMessage.Color,
				Sender = originalMessage.Sender,
				MessageContent = originalMessage.MessageContent,
			};
		}

		public bool isAlreadyRejected()
		{
			return result == MessageEventResult.Drop;
		}
	}
}
