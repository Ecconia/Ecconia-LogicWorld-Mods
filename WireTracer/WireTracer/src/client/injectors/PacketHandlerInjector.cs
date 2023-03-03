using System;
using System.Collections.Generic;
using System.Reflection;
using LogicLog;
using LogicWorld.Networking;
using LogicWorld.SharedCode.Networking;

namespace WireTracer.Client.injectors
{
	public static class PacketHandlerInjector
	{
		public static bool injectNewPacketHandler(ILogicLogger logger, IPacketHandler handler)
		{
			var fieldReceiver = typeof(GameNetwork).GetField("Receiver", BindingFlags.NonPublic | BindingFlags.Static);
			if(fieldReceiver == null)
			{
				logger.Error("Could not find field 'Receiver' in class 'GameNetwork'.");
				return false;
			}
			var fieldReceiverValue = fieldReceiver.GetValue(null);
			if(fieldReceiverValue == null)
			{
				logger.Error("Field 'Receiver' in class 'GameNetwork' was unexpectedly 'null'.");
				return false;
			}
			if(!(fieldReceiverValue is Receiver))
			{
				logger.Error("Expected value of field 'Receiver' in class 'GameNetwork' to be 'Receiver', but got: " + fieldReceiverValue.GetType());
				return false;
			}
			var fieldHandlers = typeof(Receiver).GetField("Handlers", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldHandlers == null)
			{
				logger.Error("Could not find field 'Handlers' in object 'Receiver'.");
				return false;
			}
			var fieldHandlersValue = fieldHandlers.GetValue(fieldReceiverValue);
			if(fieldHandlersValue == null)
			{
				logger.Error("Field 'Handlers' in object 'Receiver' was unexpectedly 'null'.");
				return false;
			}
			if(!(fieldHandlersValue is IDictionary<Type, IPacketHandler>))
			{
				logger.Error("Field 'Handlers' in object 'Receiver' was not of type 'IDictionary<Type, IPacketHandler>', but: " + fieldHandlersValue.GetType());
				return false;
			}
			var handlers = fieldHandlersValue as IDictionary<Type, IPacketHandler>;
			handlers.Add(handler.PacketType, handler);
			return true;
		}
	}
}
