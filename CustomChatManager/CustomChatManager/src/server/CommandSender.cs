using EccsLogicWorldAPI.Server;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Networking.Packets.Server;
using LogicAPI.Server.Networking;
using LogicWorld.Server;

namespace CustomChatManager.Server
{
	public class CommandSender
	{
		private readonly NetworkServer server;
		private readonly Connection connection;

		//Lazy load:
		private IPlayerManager playerManager;
		private string playerName;

		public CommandSender(NetworkServer server, Connection connection)
		{
			this.server = server;
			this.connection = connection;
		}

		public void sendMessage(string content)
		{
			sendMessage(content, Color24.White);
		}

		public void sendMessage(string content, Color24 color)
		{
			server.Send(connection, new ChatMessageBroadcastPacket()
			{
				Data = new ChatMessageData()
				{
					Color = color,
					Sender = null, //Messages will always be sent as system-message (until a reason against this is found).
					MessageContent = content,
				}
			});
		}

		public void broadcast(string content)
		{
			broadcast(content, Color24.White);
		}

		public void broadcast(string content, Color24 color)
		{
			server.Broadcast(new ChatMessageBroadcastPacket()
			{
				Data = new ChatMessageData()
				{
					Color = color,
					Sender = null, //Messages will always be sent as system-message (until a reason against this is found).
					MessageContent = content,
				}
			});
		}

		public string getPlayerName()
		{
			if(playerName == null)
			{
				if(playerManager == null)
				{
					playerManager = ServiceGetter.getService<IPlayerManager>();
				}
				playerName = playerManager.GetPlayerIDFromConnection(connection).Name;
			}
			return playerName;
		}

		public void sendConsoleMessage(string content)
		{
			server.Send(connection, new DebugMessagePacket()
			{
				Message = content,
			});
		}

		public void broadcastConsoleMessage(string content)
		{
			server.Broadcast(new DebugMessagePacket()
			{
				Message = content,
			});
		}
	}
}
