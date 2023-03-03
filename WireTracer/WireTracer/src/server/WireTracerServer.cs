using System;
using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Server;
using LogicAPI.Server.Networking;
using LogicWorld.Server;
using LogicWorld.SharedCode.Networking;
using WireTracer.Server.Injectors;
using WireTracer.Server.Network;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Server
{
	public class WireTracerServer : ServerMod
	{
		private readonly Dictionary<string, bool> playerHasWireTracer = new Dictionary<string, bool>();

		private NetworkServer networkServer;

		protected override void Initialize()
		{
			networkServer = Program.Get<NetworkServer>();
			if(networkServer == null)
			{
				throw new Exception("Could not get Service 'NetworkServer'.");
			}

			//Inject verifier:
			if(!ClientVerifierInjector.injectNewType(Logger, new SniffingClientVerifier(this)))
			{
				throw new Exception("Could not inject client verifier that monitors client mods.");
			}
			if(!PacketHandlerInjector.injectPacketHandler(Logger, new ClientJoinedPacketHandler(this)))
			{
				throw new Exception("Could not replace packet handler that listens to player joins.");
			}
			if(!PacketHandlerInjector.injectNewPacketHandler(Logger, new WireTracerRequestHandler(this)))
			{
				throw new Exception("Could not add packet handler for cluster listing request packet.");
			}
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

		public void playerHasMod(string playerName, bool hasWireTracer)
		{
			if(!hasWireTracer)
			{
				playerHasWireTracer.Remove(playerName);
				return;
			}
			//Change to "Info" to stalk your players. I would :P
			Logger.Debug("Player " + playerName + " uses WireTracer");
			playerHasWireTracer[playerName] = true;
		}

		public void playerJoined(PlayerData playerData, HandlerContext context)
		{
			playerHasWireTracer.TryGetValue(playerData.Name, out bool hasWireTracer);
			if(hasWireTracer)
			{
				networkServer.Send(context.Sender, new AnnounceWireTrackerPresence());
			}
		}
	}
}
