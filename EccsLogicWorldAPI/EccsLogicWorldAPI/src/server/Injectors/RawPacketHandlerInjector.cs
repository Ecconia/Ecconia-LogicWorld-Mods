using System;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicWorld.Server;
using LogicWorld.Server.Networking;
using LogicWorld.SharedCode.Networking;

namespace EccsLogicWorldAPI.Server.Injectors
{
	public static class RawPacketHandlerInjector
	{
		private static readonly Receiver receiverInstance;
		private static readonly FieldInfo field;
		private static readonly Func<Receiver, IPacketHandler[]> handlersGetter;
		
		static RawPacketHandlerInjector()
		{
			receiverInstance = Types.checkType<Receiver>(Fields.getNonNull(Fields.getPrivate(typeof(NetworkManager), "DataReceiver"), ServiceGetter.getService<INetworkManager>()));
			field = Fields.getPrivate(typeof(Receiver), "Handlers");
			handlersGetter = Delegator.createFieldGetter<Receiver, IPacketHandler[]>(field);
		}
		
		public static IPacketHandler[] getPacketHandlers()
		{
			return handlersGetter(receiverInstance);
		}
		
		public static void setPacketHandlers(IPacketHandler[] newHandlers)
		{
			//TODO: Find a better way to write 'readonly' fields, in a performant way.
			field.SetValue(receiverInstance, newHandlers);
		}
		
		public static void replacePacketHandler<T>(Func<IPacketHandler, IPacketHandler> handlerGenerator)
		{
			var handlers = getPacketHandlers();
			for(int i = 0; i < handlers.Length; i++)
			{
				if(handlers[i].PacketType == typeof(T))
				{
					handlers[i] = handlerGenerator(handlers[i]);
					return;
				}
			}
			throw new Exception("Could not find packet handler to replace for packet type '" + typeof(T).FullName + "'.");
		}
		
		public static void replacePacketHandler(IPacketHandler replacementPacketHandler)
		{
			var handlers = getPacketHandlers();
			for(int i = 0; i < handlers.Length; i++)
			{
				if(handlers[i].PacketType == replacementPacketHandler.PacketType)
				{
					handlers[i] = replacementPacketHandler;
					return;
				}
			}
			throw new Exception("Could not find packet handler to replace for packet type '" + replacementPacketHandler.PacketType.FullName + "'.");
		}
		
		public static void addPacketHandler(IPacketHandler newPacketHandler)
		{
			var oldHandlers = getPacketHandlers();
			var newHandlers = new IPacketHandler[oldHandlers.Length + 1];
			Array.Copy(oldHandlers, newHandlers, oldHandlers.Length);
			newHandlers[oldHandlers.Length] = newPacketHandler;
			setPacketHandlers(newHandlers);
		}
	}
}
