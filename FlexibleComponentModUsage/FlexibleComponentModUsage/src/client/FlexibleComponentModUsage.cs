using EccsLogicWorldAPI.Client.Injectors;
using LogicAPI.Client;
using LogicAPI.Networking.Packets.Initialization;
using LogicLog;

namespace FlexibleComponentModUsage.client
{
	public class FlexibleComponentModUsage : ClientMod
	{
		public static ILogicLogger logger;

		private RegisteredComponentManager manager;

		protected override void Initialize()
		{
			logger = Logger;
			RawPacketHandlerInjector.replacePacketHandler(oldHandler => new CustomWorldInitializationPacketHandler(this, oldHandler));
		}

		public void onWorldPacket(WorldInitializationPacket packet)
		{
			if(manager != null)
			{
				manager.checkForChanges();
			}
			else
			{
				manager = new RegisteredComponentManager();
			}
			manager.adjust(packet.ComponentIDsMap);
		}
	}
}
