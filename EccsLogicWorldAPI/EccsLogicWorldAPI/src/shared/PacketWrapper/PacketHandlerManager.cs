using System;
using System.Collections.Generic;
using LogicWorld.SharedCode.Networking;

#if LW_SIDE_SERVER
using EccsLogicWorldAPI.Server.Injectors;
#else
using EccsLogicWorldAPI.Client.Injectors;
#endif

namespace EccsLogicWorldAPI.Shared.PacketWrapper
{
	public static class PacketHandlerManager
	{
		private static readonly Dictionary<Type, ICustomPacketWrapper> allWrappers = new Dictionary<Type, ICustomPacketWrapper>();
		
		public static PacketHandlerWrapper<T> getCustomPacketHandler<T>()
		{
			if(allWrappers.TryGetValue(typeof(T), out var iWrapper))
			{
				//Other mods might overwrite the wrapped packet handler, but in that case stuff is steaming anyway.
				return (PacketHandlerWrapper<T>) iWrapper; //Dirty cast, the framework ensures the type is correct.
			}
			PacketHandlerWrapper<T> wrapper = null;
			//Do injection:
			RawPacketHandlerInjector.replacePacketHandler<T>(oldHandler => {
				wrapper = new PacketHandlerWrapper<T>((PacketHandler<T>) oldHandler);
				return wrapper;
			});
			//Do registration:
			allWrappers[typeof(T)] = wrapper;
			return wrapper;
		}
	}
}
