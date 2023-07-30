using EccsLogicWorldAPI.Server;
using LogicAPI.Networking.Packets.Initialization;
using LogicWorld.Server;
using LogicWorld.SharedCode.Networking;

namespace WireTracer.Server.Network
{
	public class ClientJoinedPacketHandler : PacketHandler<ClientLoadedWorldPacket>
	{
		private readonly WireTracerServer wireTracerServer;

		private IPlayerManager playerManager;

		public ClientJoinedPacketHandler(WireTracerServer wireTracerServer)
		{
			this.wireTracerServer = wireTracerServer;
		}

		public override void Handle(ClientLoadedWorldPacket packet, HandlerContext context)
		{
			if(playerManager == null)
			{
				//Done here, as by default this gets resolved as "lazy":
				playerManager = ServiceGetter.getService<IPlayerManager>();
				//Assume never null.
			}
			playerManager.PlayerJoinedWorld(context.Sender, packet.PlayerData);
			wireTracerServer.playerJoined(packet.PlayerData, context);
		}
	}
}
