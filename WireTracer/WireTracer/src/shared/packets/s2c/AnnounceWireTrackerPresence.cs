using LogicAPI.Networking.Packets;
using MessagePack;

namespace WireTracer.Shared.Packets.S2C
{
	[MessagePackObject]
	public class AnnounceWireTrackerPresence : Packet
	{
		[Key(0)]
		public byte unused_but_required_as_when_missing_will_crash_logic_world_on_windows = 0;
	}
}
