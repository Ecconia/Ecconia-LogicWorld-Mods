using System.Collections.Generic;
using CustomChatManager.Server.ChatServices;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Shared.PacketWrapper;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Networking.Packets.Client;
using LogicAPI.Networking.Packets.Server;
using LogicAPI.Server.Networking;
using LogicWorld.SharedCode.Networking;

namespace CustomChatManager.Server
{
	public class CustomChatManager : ICustomChatManager
	{
		public static CustomChatManager instance; //A getter for other mods.

		private readonly NetworkServer server;
		private readonly List<IChatService> chatServices = new List<IChatService>();

		public CustomChatManager()
		{
			server = ServiceGetter.getService<NetworkServer>();
			instance = this;
		}

		public void processChatPacket(ChatMessageData data, Connection sender)
		{
			ChatMessageEvent e = new ChatMessageEvent(data, sender);
			foreach(var chatService in chatServices)
			{
				chatService.processChatEvent(e);
			}

			//Forward, if not rejected:
			if(e.result == MessageEventResult.Send)
			{
				server.Broadcast(new ChatMessageBroadcastPacket()
				{
					Data = e.responseMessage
				});
			}
		}

		public void addProcessor(IChatService service)
		{
			chatServices.Add(service);
		}

		// ### Static injection code: ###

		public class CustomChatPacketHandler : CustomPacketHandler<ChatMessageSentPacket>
		{
			private readonly ICustomChatManager customChatManager;

			public CustomChatPacketHandler(ICustomChatManager customChatManager)
			{
				this.customChatManager = customChatManager;
			}

			public override void handle(ref bool isCancelled, ref ChatMessageSentPacket packet, HandlerContext context)
			{
				if(isCancelled)
				{
					return;
				}
				customChatManager.processChatPacket(packet.Data, context.Sender);
				isCancelled = true;
			}
		}
	}
}
