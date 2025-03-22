using LogicAPI;
using LogicAPI.Client;
using LogicLog;

namespace EccsLogicWorldAPI.Client
{
	public class ModClass : ClientMod
	{
		public static ILogicLogger logger;
		public static IModFiles files;
		
		protected override void Initialize()
		{
			logger = Logger;
			files = Files;
			
			// This mod provides the SyncPacket - it must be hidden until after mod-loading:
			PacketIndexOrdering.PacketIndexOrdering.init(); // This is a bit dirty, as it will perform reflection - which might fail even if ELWAPI is not used.
		}
	}
}
