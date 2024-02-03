using EccsLogicWorldAPI.Shared.PacketWrapper;
using LogicAPI.Networking.Packets.Initialization;
using LogicWorld.SharedCode.Networking;

namespace WireTracer.Server.Network
{
	public class ClientJoinedPacketHandler : CustomPacketHandler<ClientLoadedWorldPacket>
	{
		private readonly WireTracerServer wireTracerServer;
		
		public ClientJoinedPacketHandler(WireTracerServer wireTracerServer)
		{
			this.wireTracerServer = wireTracerServer;
		}
		
		public override void handle(ref bool isCancelled, ref ClientLoadedWorldPacket packet, HandlerContext context)
		{
			wireTracerServer.playerJoined(packet.PlayerData, context);
		}
	}
}
