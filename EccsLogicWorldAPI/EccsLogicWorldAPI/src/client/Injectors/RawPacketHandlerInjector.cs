using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicWorld.Networking;
using LogicWorld.SharedCode.Networking;

namespace EccsLogicWorldAPI.Client.Injectors
{
	public static class RawPacketHandlerInjector
	{
		private static readonly Receiver receiverInstance;
		private static readonly Func<Receiver, IDictionary<Type, IPacketHandler>> handlersGetter;
		
		static RawPacketHandlerInjector()
		{
			receiverInstance = Types.checkType<Receiver>(Fields.getNonNull(Fields.getPrivateStatic(typeof(GameNetwork), "Receiver")));
			var field = Fields.getPrivate(typeof(Receiver), "Handlers");
			handlersGetter = Delegator.createFieldGetter<Receiver, IDictionary<Type, IPacketHandler>>(field);
			//Cannot write to field, as it is readonly. But there is no need to anyway.
		}
		
		public static IDictionary<Type, IPacketHandler> getPacketHandlers()
		{
			return handlersGetter(receiverInstance);
		}
		
		public static void replacePacketHandler(IPacketHandler replacementPacketHandler)
		{
			var handlers = getPacketHandlers();
			if(!handlers.ContainsKey(replacementPacketHandler.PacketType))
			{
				throw new Exception("Handler for packet type '" + replacementPacketHandler.PacketType.FullName + "' cannot be replaced as it is not registered.");
			}
			handlers[replacementPacketHandler.PacketType] = replacementPacketHandler;
		}
		
		public static void addPacketHandler(IPacketHandler newPacketHandler)
		{
			var handlers = getPacketHandlers();
			if(handlers.ContainsKey(newPacketHandler.PacketType))
			{
				throw new Exception("Handler for packet type '" + newPacketHandler.PacketType.FullName + "' already exists.");
			}
			handlers[newPacketHandler.PacketType] = newPacketHandler;
		}
	}
}
