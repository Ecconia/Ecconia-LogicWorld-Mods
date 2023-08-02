using System;
using EccsLogicWorldAPI.Client.Injectors;
using LogicWorld.SharedCode.Networking;

namespace EccsLogicWorldAPI
{
	public class SideHacks
	{
		public static void hacks<T>(Func<IPacketHandler, IPacketHandler> func)
		{
			RawPacketHandlerInjector.replacePacketHandler<T>(func);
		}
	}
}
