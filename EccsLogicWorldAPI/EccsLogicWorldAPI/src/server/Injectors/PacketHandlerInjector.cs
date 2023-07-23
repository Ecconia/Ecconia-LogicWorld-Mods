using System.Reflection;
using LogicLog;
using LogicWorld.Server;
using LogicWorld.Server.Networking;
using LogicWorld.SharedCode.Networking;

namespace EccsLogicWorldAPI.Server.Injectors
{
	public static class PacketHandlerInjector
	{
		public static bool injectPacketHandler(ILogicLogger logger, IPacketHandler packetHandlerReplacement)
		{
			INetworkManager iNetworkManager = Program.Get<INetworkManager>();
			if(iNetworkManager == null)
			{
				logger.Error("Could not get service INetworkManager.");
				return false;
			}
			if(iNetworkManager.GetType() != typeof(NetworkManager))
			{
				logger.Error("Service INetworkManager is not a NetworkManager class.");
				return false;
			}
			IReceiver iReceiver = (IReceiver) typeof(NetworkManager)
			                                  .GetField("DataReceiver", BindingFlags.NonPublic | BindingFlags.Instance)
			                                  .GetValue(iNetworkManager);
			if(iReceiver == null)
			{
				logger.Error("Failed to get IReceiver from NetworkManager.");
				return false;
			}
			if(iReceiver.GetType() != typeof(Receiver))
			{
				logger.Error("Interface IReceiver is not a Receiver class.");
				return false;
			}
			IPacketHandler[] handlers = (IPacketHandler[]) typeof(Receiver)
			                                               .GetField("Handlers", BindingFlags.NonPublic | BindingFlags.Instance)
			                                               .GetValue(iReceiver);
			if(handlers == null)
			{
				logger.Error("Failed to get IPacketHandler[] from Receiver.");
				return false;
			}
			for(int i = 0; i < handlers.Length; i++)
			{
				if(handlers[i].PacketType == packetHandlerReplacement.PacketType)
				{
					logger.Info("Found the Handler for '" + packetHandlerReplacement.PacketType + "', replacing it!");
					handlers[i] = packetHandlerReplacement;
					return true;
				}
			}
			logger.Error("Not able to find the IPacketHandler in charge of client chat messages.");
			return false;
		}
	}
}
