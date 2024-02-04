using EccsLogicWorldAPI.Server;
using LogicWorld.Server;

namespace CustomChatManager.Server.ChatServices
{
	public class VerifySenderName : IChatService
	{
		private readonly IPlayerManager playerManager;
		
		public VerifySenderName()
		{
			playerManager = ServiceGetter.getService<IPlayerManager>();
		}
		
		public void processChatEvent(ChatMessageEvent e)
		{
			if(e.isAlreadyRejected())
			{
				return;
			}
			var playerID = playerManager.GetPlayerIDFromConnection(e.sender);
			var claimedSenderName = e.originalMessage.Sender;
			if(!playerID.Name.Equals(claimedSenderName))
			{
				var appearanceData = playerManager.GetPlayerDataFromConnection(e.sender).AppearanceData;
				e.responseMessage.Color = appearanceData.BackgroundColor;
				e.responseMessage.Sender = playerID.Name;
				e.responseMessage.MessageContent = "<" + claimedSenderName + "> " + e.originalMessage.MessageContent;
			}
		}
	}
}
