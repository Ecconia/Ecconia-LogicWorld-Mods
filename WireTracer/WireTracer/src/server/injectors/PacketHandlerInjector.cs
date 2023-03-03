using System;
using System.Reflection;
using LogicLog;
using LogicWorld.Server;
using LogicWorld.Server.Networking;
using LogicWorld.SharedCode.Networking;

//Bluntly stolen code from one of my other projects.
//TODO: Use a dependency for this class instead of providing it like this.
namespace WireTracer.Server.Injectors
{
	public static class PacketHandlerInjector
	{
		private static (object receiver, FieldInfo field, IPacketHandler[])? getValidatedResources(ILogicLogger logger)
		{
			var iNetworkManager = Program.Get<INetworkManager>();
			if(iNetworkManager == null)
			{
				logger.Error("Could not get service INetworkManager.");
				return null;
			}
			if(iNetworkManager.GetType() != typeof(NetworkManager))
			{
				logger.Error("Service INetworkManager is not of type 'NetworkManager'.");
				return null;
			}
			var fieldDataReceiver = typeof(NetworkManager).GetField("DataReceiver", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldDataReceiver == null)
			{
				logger.Error("Could not find field 'DataReceiver' in class 'NetworkManager'.");
				return null;
			}
			var fieldDataReceiverValue = fieldDataReceiver.GetValue(iNetworkManager);
			if(fieldDataReceiverValue == null)
			{
				logger.Error("Field 'DataReceiver' in class 'NetworkManager' is 'null'.");
				return null;
			}
			if(fieldDataReceiverValue.GetType() != typeof(Receiver))
			{
				logger.Error("Value of field 'DataReceiver' in class 'NetworkManager' is not of type 'Receiver', but: " + fieldDataReceiverValue.GetType());
				return null;
			}
			var fieldHandlers = typeof(Receiver).GetField("Handlers", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldHandlers == null)
			{
				logger.Error("Could not find field 'Handlers' in class 'Receiver'.");
				return null;
			}
			var handlers = (IPacketHandler[]) fieldHandlers.GetValue(fieldDataReceiverValue);
			if(handlers == null)
			{
				logger.Error("Field 'Handlers' in class 'Receiver' is 'null'.");
				return null;
			}
			return (fieldDataReceiverValue, fieldHandlers, handlers);
		}

		public static bool injectNewPacketHandler(ILogicLogger logger, IPacketHandler newPacketHandler)
		{
			var validatedResources = getValidatedResources(logger);
			if(!validatedResources.HasValue)
			{
				return false;
			}
			var (receiver, field, handlers) = validatedResources.Value;

			Array.Resize(ref handlers, handlers.Length + 1);
			handlers[handlers.Length - 1] = newPacketHandler;
			field.SetValue(receiver, handlers);
			logger.Debug("Added handler for packet type '" + newPacketHandler.PacketType + "'!");
			return true;
		}

		public static bool injectPacketHandler(ILogicLogger logger, IPacketHandler packetHandlerReplacement)
		{
			var validatedResources = getValidatedResources(logger);
			if(!validatedResources.HasValue)
			{
				return false;
			}
			var (_, _, handlers) = validatedResources.Value;

			for(int i = 0; i < handlers.Length; i++)
			{
				if(handlers[i].PacketType == packetHandlerReplacement.PacketType)
				{
					logger.Debug("Found the Handler for '" + packetHandlerReplacement.PacketType + "', replacing it!");
					handlers[i] = packetHandlerReplacement;
					return true;
				}
			}
			logger.Error("Not able to find the IPacketHandler in charge of client chat messages.");
			return false;
		}
	}
}
