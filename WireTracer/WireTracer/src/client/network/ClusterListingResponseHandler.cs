using LogicWorld.SharedCode.Networking;
using WireTracer.Client.Tool;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Client.Network
{
	public class ClusterListingResponseHandler : PacketHandler<ClusterListingResponse>
	{
		public override void Handle(ClusterListingResponse packet, HandlerContext context)
		{
			WireTracerTool.onResponseReceived(packet);
		}
	}
}
