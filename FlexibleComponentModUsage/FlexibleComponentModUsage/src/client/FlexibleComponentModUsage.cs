using System;
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
			hijackPacketHandler();
			//If you plan to do anything after this, make the hijack method return bool and stop here on error.
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

		private void hijackPacketHandler()
		{
			//TODO: use callback on wrapped packet handler instead
			var handlers = RawPacketHandlerInjector.getPacketHandlers();
			if(!handlers.TryGetValue(typeof(WorldInitializationPacket), out var oldHandler))
			{
				throw new Exception("There is no PacketHandler registered for the WorldInitializationPacket.");
			}
			handlers[typeof(WorldInitializationPacket)] = new CustomWorldInitializationPacketHandler(this, oldHandler);
		}
	}
}
