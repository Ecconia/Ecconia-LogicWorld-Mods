using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using EccsLogicWorldAPI.Server.AccessHelpers;
using EccsLogicWorldAPI.Server.Injectors;
using EccsLogicWorldAPI.Shared.AccessHelper;
using EccsLogicWorldAPI.Shared.PacketIndexOrdering;
using EccsLogicWorldAPI.Shared.PacketWrapper;
using Lidgren.Network;
using LogicAPI.Networking.Packets.Initialization;
using LogicAPI.Server.Networking;
using LogicAPI.Server.Networking.ClientVerification;
using LogicWorld.Server.Networking.Implementation;
using LogicWorld.SharedCode.Networking;
using LogicWorld.SharedCode.Saving;
using LogicWorld.SharedCode.Saving.Data;
using Shared_Code.Code.Networking;

namespace EccsLogicWorldAPI.Server.PacketIndexOrdering
{
	public static class PacketIndexOrdering
	{
		private static readonly NetworkServer sender;
		private static readonly NetServer lidgren;
		
		/// <summary> A timestamp of when the lidgren handshakes got cleaned up for the last time. Prevents memory overflow. </summary>
		private static long lastCleanup;
		/// <summary> A quick and dirty access into Lidgrens Handshakes storage, used to check which network connection is no longer relevant. </summary>
		private static Dictionary<IPEndPoint, NetConnection> lidgrenHandshakeConnections;
		
		static PacketIndexOrdering()
		{
			sender = ServiceGetter.getService<NetworkServer>();
			lidgren = Types.checkType<NetServer>(Fields.getNonNull(Fields.getPrivate(typeof(LidgrenNetworkServer), "Lidgren"), sender));
		}
		
		/// <summary> Store assemblies which contain optional packets. Used to inject them into the packet-id map again. </summary>
		private static readonly HashSet<Assembly> optionalAssemblies = [];
		/// <summary>  </summary>
		private static readonly HashSet<IPEndPoint> connectionsRequestingPacketSyncing = [];
		
		/// <summary> A flag that gets set once the framework has been "enabled". Only used during mod loading phase. </summary>
		private static bool isEnabled;
		/// <summary> Cache of the synchronization packet, sent to all clients using this framework. </summary>
		private static SyncPacketIDPacket syncPacket;
		
		// API/Init code:
		
		internal static void init()
		{
			PacketDeltaDebugger.createInitial(MetaMods.getAllMetaMods.ToList());
			// This mod provides the SyncPacket - it must be hidden until after mod-loading:
			PacketIndexHelper.removePacketsOfAssembly(typeof(PacketIndexOrdering).Assembly, out _);
			_dbg();
		}
		
		/// <summary> Will mark the packets of a mod as optional. This enables packet-ID map syncing between client &amp; server, ensuring packets to have the correct ID. </summary>
		/// <remarks> Will remove the optional packets from the packet map until after all mods are loaded. </remarks>
		/// <param name="assembly"> The assembly which provides packets. </param>
		public static void markModAsOptional(Assembly assembly)
		{
			_dbg();
			// We got at least one mod, which requests packet syncing. Prime callbacks and other things (enable this framework):
			enable();
			// Remove the optional packets of that mod's assembly:
			PacketIndexHelper.removePacketsOfAssembly(assembly, out _);
			_dbg();
			// Remember the assembly though, so that it can be re-added later:
			optionalAssemblies.Add(assembly);
		}
		
		// Private/Internal prime/setup code:
		
		private static void enable()
		{
			// Only enable this framework once:
			if(isEnabled)
			{
				return;
			}
			isEnabled = true;
			
			// The save/world is loaded after all mods. We can (ab)use this callback to have reliable code execution after all mods (and their packets) are loaded.
			// This callback will create the correct packet-id map including all optional packets in the right order.
			SaveReader.PostParseSaveTransformers += primeServerPackets;
			
			// Registers a verifier, which checks if any of the clients requested for this framework to send a packet synchronization.
			// This is the case, when the client has mods with optional packets installed.
			RawJoinVerifierInjector.addVerifier(new ModSniffer());
			
			// To send the synchronization packet to a player, we need to know when they are about to join.
			// Specifically we want to send synchronization packets, before any other mod can do it. So basically before everything else.
			// Thus, right after the connection was established is the best time to send the packet synchronization packet.
			PacketHandlerManager.getCustomPacketHandler<ConnectionEstablishedPacket>()
				.addHandlerToFront(new CustomConnectionEstablishedHandler());
		}
		
		private static void primeServerPackets(SaveFileData saveFileData)
		{
			// This is only meant to run once, it is an initialization method. Thus remove the callback right away:
			SaveReader.PostParseSaveTransformers -= primeServerPackets;
			
			// In the current state the packet-id map only contains non-optional packets.
			// As the very first packet after that we want to have the synchronization packet, thus load the assembly of this mod first.
			PacketManager.LoadPacketsIn(typeof(PacketIndexOrdering).Assembly);
			_dbg();
			
			// After that, add all optional packets from the mods in no particular order. The synchronization packet ensures that the client has the correct packet IDs.
			foreach(var assembly in optionalAssemblies)
			{
				PacketManager.LoadPacketsIn(assembly);
				_dbg();
			}
			
			// Once all packets are registered, create the synchronization packet. It will always be the same packet, thus create and cache it once:
			syncPacket = createSyncPacket();
			
			// To properly remove data of failed connections, we cannot listen to a disconnect event. But instead have to check the internals of Lidgren - yay~
			lidgrenHandshakeConnections = Types.checkType<Dictionary<IPEndPoint, NetConnection>>(
				Fields.getNonNull(
					Fields.getPrivate(typeof(NetPeer), "m_handshakes"),
					lidgren
				)
			);
			lastCleanup = Stopwatch.GetTimestamp();
		}
		
		private static SyncPacketIDPacket createSyncPacket()
		{
			// The synchronization packets transfers a list of mandatory (non-optional) and optional packets with their IDs.
			var mandatoryPackets = new List<(string, ushort)>();
			var optionalPackets = new List<(string, ushort)>();
			// It generates it from the currently being-used packet map with all packets.
			var packetIdMap = PacketIndexHelper.getPacketsDictionary();
			
			// Mandatory packets MUST be before all optional packets. This is by design - else this framework did bad job :/
			// To make sure this won't ever fail for any reason, validate this here.
			var currentlyProcessingOptionalPackets = false; // Flag to ensure correct packet order.
			foreach(var (index, type) in packetIdMap.Forwards)
			{
				if(!optionalAssemblies.Contains(type.Assembly))
				{
					// Mandatory packet
					if(currentlyProcessingOptionalPackets)
					{
						throw new Exception("Wrongly constructed packet-ID map. Mandatory packet encountered after an optional packet.");
					}
					mandatoryPackets.Add((type.FullName, index));
				}
				else
				{
					// Optional packet
					currentlyProcessingOptionalPackets = true; // From now on only process optional packets, else error.
					optionalPackets.Add((type.FullName, index));
				}
			}
			
			return new SyncPacketIDPacket {
				mandatoryPackets = mandatoryPackets,
				optionalPackets = optionalPackets,
			};
		}
		
		// Player join callbacks code:
		
		private class ModSniffer : IClientVerifier
		{
			public void Verify(VerificationContext ctx)
			{
				// At this point the only thing reliably identifying a user is its endpoint - the username might be faked. Thus use the endpoint instead.
				var endPoint = ctx.RemoteConnection.RemoteEndPoint;
				// Check if the client has the fake mod installed, which indicates that it requests packet synchronization:
				if(ctx.ApprovalPacket.ClientMods.Contains(SyncPacketIDPacket.FakeModName))
				{
					// Packet synchronization is requested!
					connectionsRequestingPacketSyncing.Add(endPoint); // Remember connection
					
					// We got another connection requesting packet synchronization.
					// In case that there are like thousands of them which all fail to join, we do not want to risk a memory overflow. Thus clean them up:
					cleanupRememberedSynchronizationConnections();
				}
				else
				{
					// It is unlikely, that this end-point is used twice in a row - but just to be sure - ensure that it won't be remembered - we don't want to crash random players.
					connectionsRequestingPacketSyncing.Remove(endPoint);
				}
			}
			
			private static void cleanupRememberedSynchronizationConnections()
			{
				// Prevent the cleanup from running more than once per minute:
				var now = Stopwatch.GetTimestamp();
				var difference = now - lastCleanup;
				var difference_s = difference / Stopwatch.Frequency;
				if(difference_s >= 60)
				{
					return; // Not yet time to clean up - wait a minute
				}
				lastCleanup = now;
				
				// Remove all connections, which Lidgren does not "know" anymore. They are effectively disconnected.
				var entriesToRemove = connectionsRequestingPacketSyncing.Where(
						// Lidgren stores all connection endpoints in a handshake dictionary, until they either joined or disconnected. Query their presence:
						endpoint => !lidgrenHandshakeConnections.ContainsKey(endpoint)
					).ToList();
				entriesToRemove.ForEach(key => connectionsRequestingPacketSyncing.Remove(key));
			}
		}
		
		private class CustomConnectionEstablishedHandler : CustomPacketHandler<ConnectionEstablishedPacket>
		{
			public override void handle(ref bool isCancelled, ref ConnectionEstablishedPacket packet, HandlerContext context)
			{
				if(isCancelled)
				{
					return;
				}
				
				// Check if we remembered this connection to have requested packet synchronization.
				// As a side effect, if it did - remove it from the remembered storage to keep things clean.
				if(!connectionsRequestingPacketSyncing.Remove(context.SenderEndPoint))
				{
					return;
				}
				
				//Client is in remember storage, send sync packet:
				sender.Send(context.Sender, syncPacket);
			}
		}
		
		private static void _dbg()
		{
			PacketDeltaDebugger.printAndCreateDelta();
		}
	}
}
