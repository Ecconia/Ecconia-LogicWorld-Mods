using LogicWorld.SharedCode.Networking;
using WireTracer.Shared.Packets.C2S;

namespace WireTracer.Server.Network
{
	public class WireTracerRequestHandler : PacketHandler<RequestClusterListing>
	{
		private readonly WireTracerServer wireTracerServer;

		public WireTracerRequestHandler(WireTracerServer wireTracerServer)
		{
			this.wireTracerServer = wireTracerServer;
		}

		public override void Handle(RequestClusterListing packet, HandlerContext context)
		{
			wireTracerServer.playerRequestsCluster(context.Sender, packet.requestGuid, packet.pegAddress);
		}
	}
}
