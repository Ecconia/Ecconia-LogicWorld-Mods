using CustomChatManager.Server.ChatServices;
using LogicAPI.Data;
using LogicAPI.Networking;

namespace CustomChatManager.Server
{
	public interface ICustomChatManager
	{
		void processChatPacket(ChatMessageData packetData, Connection contextSender);

		void addProcessor(IChatService service);
	}
}
