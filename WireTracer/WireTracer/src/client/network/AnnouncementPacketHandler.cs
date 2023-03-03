using LogicWorld.SharedCode.Networking;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Client.network
{
	public class AnnouncementPacketHandler : PacketHandler<AnnounceWireTrackerPresence>
	{
		public override void Handle(AnnounceWireTrackerPresence packet, HandlerContext context)
		{
			WireTracer.logger.Debug("WireTracer support available on server.");
			WireTracer.serverHasWireTracer = true;
		}
	}
}
