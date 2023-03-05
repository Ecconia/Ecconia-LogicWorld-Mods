using System;
using System.Collections.Generic;
using System.Reflection;
using LogicAPI.Client;
using LogicAPI.Networking.Packets.Initialization;
using LogicLog;
using LogicWorld.Networking;
using LogicWorld.SharedCode.Networking;

namespace FlexibleComponentModUsage.client
{
	public class FlexibleComponentModUsage : ClientMod
	{
		public const string error = " This mod will not be functional. You will not get components sorted on join. Please notify the plugin maintainer.";
		public static ILogicLogger logger;

		private RegisteredComponentManager manager;

		protected override void Initialize()
		{
			logger = Logger;
			//Do da reflection thing:
			if(RegisteredComponentManager.hasInitializationFailed())
			{
				return; //Nothing to do now.
			}
			hijackPacketHandler(Logger);
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

		private void hijackPacketHandler(ILogicLogger logger)
		{
			var fieldReceiver = typeof(GameNetwork).GetField("Receiver", BindingFlags.NonPublic | BindingFlags.Static);
			if(fieldReceiver == null)
			{
				logger.Error("Could not find field 'Receiver' in class 'GameNetwork'." + error);
				return;
			}
			var fieldReceiverValue = fieldReceiver.GetValue(null);
			if(fieldReceiverValue == null)
			{
				logger.Error("Field 'Receiver' in class 'GameNetwork' was unexpectedly 'null'." + error);
				return;
			}
			if(!(fieldReceiverValue is Receiver receiver))
			{
				logger.Error("Instance of public 'IReceiver' is not of type 'Receiver', but '" + fieldReceiverValue.GetType() + "'." + error);
				return;
			}
			var fieldHandlers = typeof(Receiver).GetField("Handlers", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldHandlers == null)
			{
				logger.Error("Could not find field 'Handlers' in object 'Receiver'." + error);
				return;
			}
			var fieldHandlersValue = fieldHandlers.GetValue(receiver);
			if(fieldHandlersValue == null)
			{
				logger.Error("Field 'Handlers' in object 'Receiver' was unexpectedly 'null'." + error);
				return;
			}
			if(!(fieldHandlersValue is IDictionary<Type, IPacketHandler> handlers))
			{
				logger.Error("Field 'Handlers' in object 'Receiver' was not of type 'IDictionary<Type, IPacketHandler>', but: '" + fieldHandlersValue.GetType() + "'." + error);
				return;
			}

			//Iterate over handlers:
			if(!handlers.TryGetValue(typeof(WorldInitializationPacket), out var oldHandler))
			{
				logger.Error("There is no PacketHandler registered for the WorldInitializationPacket." + error);
				return;
			}
			handlers[typeof(WorldInitializationPacket)] = new CustomWorldInitializationPacketHandler(this, oldHandler);
		}
	}
}
