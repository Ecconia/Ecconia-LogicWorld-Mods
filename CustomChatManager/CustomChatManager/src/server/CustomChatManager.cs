using System;
using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Networking.Packets.Client;
using LogicAPI.Networking.Packets.Server;
using LogicAPI.Server.Networking;
using LogicLog;
using LogicWorld.Server;
using LogicWorld.SharedCode.Networking;
using ServerModdingSuit.Server;

namespace CustomChatManager.Server
{
	public class CustomChatManager : ICustomChatManager
	{
		public static CustomChatManager instance; //A getter for other mods.

		private readonly NetworkServer server;
		private readonly List<IChatService> chatServices = new List<IChatService>();

		public CustomChatManager()
		{
			server = Program.Get<NetworkServer>();
			if(server == null)
			{
				throw new Exception("Could not get NetworkServer.");
			}

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
		
		public static void inject(ILogicLogger logger, ICustomChatManager customChatManager)
		{
			//Ignore success.
			PacketHandlerInjector.injectPacketHandler(logger, new CustomChatPacketHandler(customChatManager));
		}
		
		private class CustomChatPacketHandler : PacketHandler<ChatMessageSentPacket>
		{
			private readonly ICustomChatManager customChatManager;

			public CustomChatPacketHandler(ICustomChatManager customChatManager)
			{
				this.customChatManager = customChatManager;
			}

			public override void Handle(ChatMessageSentPacket packet, HandlerContext context)
			{
				customChatManager.processChatPacket(packet.Data, context.Sender);
			}
		}
	}
}
