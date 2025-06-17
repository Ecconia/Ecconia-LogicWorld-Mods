using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using EccsLogicWorldAPI.Shared.PacketIndexOrdering;
using JimmysUnityUtilities.Collections;
using LogicAPI;
using LogicAPI.Client.Networking;
using LogicAPI.Modding;
using LogicAPI.Networking.Packets.Initialization;
using LogicWorld.Networking;
using LogicWorld.SharedCode.Modding;
using Shared_Code.Code.Networking;
using UnityEngine.SceneManagement;

namespace EccsLogicWorldAPI.Client.PacketIndexOrdering
{
	public static class PacketIndexOrdering
	{
		/// <summary> A flag that gets set once the framework has been "enabled". Only used during mod loading phase. </summary>
		private static bool isEnabled;
		/// <summary> The network client is not available on mod loading and has to be set later. Used to handle packets before world-loading. </summary>
		private static NetworkClient networkClient;
		/// <summary> This framework injects a fake mod during server-connection phase, to signal the server that packets should be synchronized. Cached as it is tricky to create. </summary>
		private static MetaMod fakeMod;
		
		/// <summary> Stores all optional (and previously removed) packets in a string=>Type format. Allows to easily re-add them to the packet ID map later on. </summary>
		private static readonly Dictionary<string, Type> optionalAndRemovedPackets = new Dictionary<string, Type>();
		/// <summary> Stores a set of all the types of mandatory packets (and the synchronization packet), used as reference when applying the server packet map. </summary>
		private static HashSet<Type> packetTypesBackup;
		/// <summary> A lookup for mandatory packets. Created as the server only provides strings instead of types. </summary>
		private static Dictionary<string, Type> mandatoryPacketTypeLookup;
		/// <summary> Stores all optional packets available on the server and client. This allows client mods with optional packets to check support of their features on the server. </summary>
		private static readonly HashSet<Type> optionalPacketsSupportedByServer = [];
		
		public static void init()
		{
			PacketDeltaDebugger.createInitial();
			// This mod provides the SyncPacket - it must be hidden until after mod-loading:
			PacketIndexHelper.removePacketsOfAssembly(typeof(PacketIndexOrdering).Assembly, out _);
			_dbg();
		}
		
		/// <summary> Lookup to check if the server supports a certain packet type. </summary>
		/// <param name="packetType"> The packet type to check existence for. </param>
		/// <returns> True, if the server supports provided packet type. </returns>
		public static bool doesServerSupportPacket(Type packetType)
			=> optionalPacketsSupportedByServer.Contains(packetType);
		
		/// <summary> Will mark the packets of a mod as optional. This enables packet-ID map syncing between client &amp; server, ensuring packets to have the correct ID. </summary>
		/// <remarks> Will remove the optional packets from the packet map. Reads them with correct ID when the server supports them. </remarks>
		/// <param name="assembly"> The assembly which provides packets. </param>
		public static void markModAsOptional(Assembly assembly)
		{
			_dbg();
			// Enable this framework, this is only done once:
			enable();
			// Remove the packets for the mod assembly:
			PacketIndexHelper.removePacketsOfAssembly(assembly, out var removedPackets);
			_dbg();
			// But remember which packets got removed. This makes re-adding them later on much easier / possible:
			removedPackets.ForEach(removedType => optionalAndRemovedPackets.Add(removedType.FullName!, removedType));
		}
		
		private static void enable()
		{
			// Only enable this framework once:
			if(isEnabled)
			{
				return;
			}
			isEnabled = true;
			
			// Create and cache a fake mod. This mod is used to send a "message" to the server, via the list of installed mods.
			// The servers client verifier can then remember which client intends to synchronize packets.
			var constructor = typeof(MetaMod).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
			fakeMod = (MetaMod) constructor.Invoke([
				ModClass.files, // Just provide some files, it really does not matter.
				new FakeModManifest(),
				false,
				ModFormat.Folder
			]);
			
			// This callback will only ever be called once. It sets up things after all mods are loaded.
			// This happened to be before joining a save. But it can be done even earlier.
			GameNetwork.OnBeforeConnect += performPriming;
			
			// Every time before connecting to a server, we have to register some callbacks and inject a fake mod.
			// This will allow the synchronization packet to be received and more...
			GameNetwork.OnBeforeConnect += handleBeforeConnect;
		}
		
		private static void performPriming()
		{
			// We only want to prime things once, thus remove this callback right away:
			GameNetwork.OnBeforeConnect -= performPriming;
			
			// The network client is only setup after mod loading, remember it for later use.
			networkClient = Types.checkType<NetworkClient>(Properties.getPrivateStatic(typeof(GameNetwork), "Client").GetValue(null));
			
			// We want to register the packet synchronization packet again as the first optional packet, or the last mandatory packet:
			PacketManager.LoadPacketsIn(typeof(PacketIndexOrdering).Assembly);
			_dbg();
			
			// Store a set all the original mandatory packets (and the synchronization packet).
			// At this point all optional packets are removed. And the synchronization packet was added.
			// Do not store a reference, as the original will be further modified.
			packetTypesBackup = PacketIndexHelper.getPacketsDictionary().Forwards.Values.ToHashSet();
			// The server only provides names, thus create a mapping from Name=>Type for all mandatory packets:
			mandatoryPacketTypeLookup = packetTypesBackup.ToDictionary(type => type.FullName);
		}
		
		private static void handleBeforeConnect()
		{
			// When joining a server, reset this list. It will be filled again when the server sends a packet synchronization packet.
			// If the server doesn't send one - it has no optional packets, thus this set stays empty.
			optionalPacketsSupportedByServer.Clear();
			
			// Make sure that when a scene loads, we always remove all callbacks we added.
			// In case of crashes (and a scene switches) we do not want any lingering callbacks.
			SceneManager.sceneLoaded += cleanupCallbacksAndOther;
			
			// Before connecting to a server, we want to announce that we support packet map synchronization.
			// For that add a fake-mod (ID) to the mod list, which the server can then see and evaluate.
			ModRegistryInternal.LoadedModsByName.Add(fakeMod.Manifest.ID, fakeMod); // This works reliable, is very sketchy though (like this whole API).
			
			// No need to add a packet-handler, as the SyncPacket is dropped by LW anyway - as it is sent before the WorldPacket
			// Instead, let's capture the packet before LWs main processing code runs - our packet must be fully processed
			// before any other packet is deserialized anyway!
			networkClient.PacketReceived += handleStartupPackets;
		}
		
		private static void cleanupCallbacksAndOther(Scene scene, LoadSceneMode mode)
		{
			// We are expecting the EmptyScene to be loaded before joining a server.
			// Any other scene would be a problem and should cause a cleanup.
			if("EmptyScene".Equals(scene.name))
			{
				return;
			}
			
			// We only run this once, after the callbacks are no longer relevant.
			SceneManager.sceneLoaded -= cleanupCallbacksAndOther;
			
			// Remove the fake mod, at this point it served its purpose.
			ModRegistryInternal.LoadedModsByName.Remove(fakeMod.Manifest.ID);
			
			// This was already removed, or we crashed.
			networkClient.PacketReceived -= handleStartupPackets;
		}
		
		private static void handleStartupPackets(ushort typeCode, object packet, ClientConnection sender)
		{
			// We only need to listen to two packets:
			// - The Synchronization Packet, which is what this framework is written for.
			// - The World Initialization Packet, which means that the server has no optional Packets and is ready to proceed from the connection phase.
			switch (packet)
			{
				case SyncPacketIDPacket synchronizationPacket:
					networkClient.PacketReceived -= handleStartupPackets; // Remove callback, we got the packet we intended to receive. No more packet monitoring required.
					doThePacketMagic(synchronizationPacket);
					return;
				
				// If we got a world initialization packet, then we missed or never gotten the sync packet. In that case assume the server does not have optional packets.
				case WorldInitializationPacket:
					networkClient.PacketReceived -= handleStartupPackets; // Remove callback, the purpose of this method is over. No syncing packet received.
					return;
			}
		}
		
		private static void doThePacketMagic(SyncPacketIDPacket syncPacket)
		{
			// syncPacket.debugPrint();
			
			/*
			 * In general, construct a new packet list which is for the most part identical to what the server sent.
			 * Ignore indices for the most part, they're just going be reconstructed.
			 *
			 * We assume that at this point the old packet list does only contain mandatory packets!
			 * - If the server declares a mandatory packet, which is not on the client => Error!
			 * - If the client contains a mandatory packet, which the server does not declare => Error!
			 *
			 * There should be no need to validate, that the SyncPacket packet is the last mandatory packet,
			 *  as at this point it was already parsed properly. And the client map MUST only contain ALL mandatory packets.
			 *
			 * Do not exactly trust the server. Validate, that no duplicates are in the new packet map.
			 */
			
			var oldPacketList = new HashSet<Type>(packetTypesBackup); // Copy the set, to allow removing entries from it.
			var newPacketList = new TwoWayDictionary<ushort, Type>();
			
			// First add all mandatory packets (and perform validation):
			foreach(var (typeName, targetIndex) in syncPacket.mandatoryPackets)
			{
				// Get actual type for type-name:
				if(!mandatoryPacketTypeLookup.TryGetValue(typeName, out var type))
				{
					throw new Exception($"Server expects client to have packet '{typeName}' enabled/installed, but the client does not have it. Wrong mod versions? Consult Ecconia or so...");
				}
				// Remove the entry from the previous list, to find remainders.
				oldPacketList.Remove(type); // This will work, as the name-lookup was generated from it.
				
				// Add the entry to the new map:
				if(!newPacketList.TryAdd(targetIndex, type))
				{
					throw new Exception("Server sent malformed packet-ID map sync packet. Entry for type OR index was already present.");
				}
			}
			
			// Then add all optional packets (with less validation):
			foreach(var (typeName, targetIndex) in syncPacket.optionalPackets)
			{
				if(mandatoryPacketTypeLookup.ContainsKey(typeName))
				{
					throw new Exception($"Client has an optional packet '{typeName}' activated (as mandatory), but the server intends this to be an optional packet. Wrong mod versions?");
				}
				
				if(!optionalAndRemovedPackets.TryGetValue(typeName, out var type))
				{
					// This optional packet is only known to the server. The client does not have it installed.
					continue;
				}
				
				// Add the entry to the new map:
				if(!newPacketList.TryAdd(targetIndex, type))
				{
					throw new Exception("Server sent malformed packet-ID map sync packet. Entry for type OR index was already present.");
				}
				// Remember this type, so that mods can query if their packet is supported.
				optionalPacketsSupportedByServer.Add(type);
			}
			
			// There should be no packet remaining in the set of mandatory packets.
			// If something is remaining, the client has more mandatory packets than the server => clash - may never happen.
			if(oldPacketList.Any())
			{
				ModClass.logger.Error("List of mandatory packets present on client, which should not exist:\n- '" + string.Join("'\n- '", oldPacketList.Select(type => type.FullName)));
				throw new Exception("Client has more mandatory packets installed than the server assumes. Got non-optional mod? Got wrong mod version? (See console for list of packets).");
			}
			
			// Finally, update LWs packet-id map with the new mapping:
			PacketIndexHelper.setPacketsHighestIndex(newPacketList.Forwards.Keys.Max());
			PacketIndexHelper.setPacketsDictionary(newPacketList);
			_dbg();
		}
		
		private class FakeModManifest : ModManifest
		{
			public FakeModManifest()
			{
				ID = SyncPacketIDPacket.FakeModName;
			}
		}
		
		private static void _dbg()
		{
			PacketDeltaDebugger.printAndCreateDelta();
		}
	}
}
