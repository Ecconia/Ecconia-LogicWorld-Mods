using LogicAPI.Server;

namespace EccsLogicWorldAPI.Server
{
	public class ModClass : ServerMod
	{
		protected override void Initialize()
		{
			// This mod provides the SyncPacket - it must be hidden until after mod-loading:
			PacketIndexOrdering.PacketIndexOrdering.init(); // This is a bit dirty, as it will perform reflection - which might fail even if ELWAPI is not used.
		}
	}
}
