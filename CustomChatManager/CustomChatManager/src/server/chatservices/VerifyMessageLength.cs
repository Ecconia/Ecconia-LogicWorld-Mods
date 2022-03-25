using System;
using LogicWorld.Server;

namespace CustomChatManager.Server.ChatServices
{
	public class VerifyMessageLength : IChatService
	{
		private readonly IGameConfig config;

		public VerifyMessageLength()
		{
			config = Program.Get<IGameConfig>();
			if(config == null)
			{
				throw new Exception("Could not get IGameConfig.");
			}
		}

		public void processChatEvent(ChatMessageEvent e)
		{
			int messageLengthLimit = config.Values.ChatMessageLengthLimit;
			if(
				messageLengthLimit < 0 ||
				messageLengthLimit > 0 &&
				messageLengthLimit < (e.originalMessage.Sender.Length + e.originalMessage.MessageContent.Length)
			)
			{
				//Discard packet!
				e.result = MessageEventResult.Drop;
			}
		}
	}
}
