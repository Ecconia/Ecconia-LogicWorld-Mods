using System;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Server.Injectors;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Server;
using LogicAPI.Server.Networking;
using WireTracer.Server.Network;

namespace WireTracer.Server
{
	public class WireTracerServer : ServerMod
	{
		private NetworkServer networkServer;
		
		protected override void Initialize()
		{
			networkServer = ServiceGetter.getService<NetworkServer>();
			
			//Inject verifier:
			RawPacketHandlerInjector.addPacketHandler(new WireTracerRequestHandler(this));
		}
		
		public void playerRequestsCluster(Connection sender, Guid packetRequestGuid, PegAddress origin)
		{
			if(!ClusterCollector.collect(origin, out var response))
			{
				return; //Failed to connect, ignore.
			}
			response.requestGuid = packetRequestGuid;
			networkServer.Send(sender, response);
		}
	}
}
