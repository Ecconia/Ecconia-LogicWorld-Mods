using System;
using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using JimmysUnityUtilities.Collections;
using LICC;
using Shared_Code.Code.Networking;

namespace EccsLogicWorldAPI.Shared.PacketIndexOrdering
{
	public static class PacketIndexHelper
	{
		public static readonly Func<TwoWayDictionary<ushort, Type>> getPacketsDictionary;
		public static readonly Func<ushort> getPacketsHighestIndex;
		//TBI: Its readonly, thus my delegator code does not work...
		// private static readonly Action<TwoWayDictionary<ushort, Type>> setPacketsDictionary;
		private static readonly FieldInfo fieldDictionary;
		public static readonly Action<ushort> setPacketsHighestIndex;
		
		static PacketIndexHelper()
		{
			fieldDictionary = Fields.getPrivateStatic(typeof(PacketManager), "PacketTypes");
			var fieldIndex = Fields.getPrivateStatic(typeof(PacketManager), "PacketIDCounter");
			
			getPacketsDictionary = Delegator.createStaticFieldGetter<TwoWayDictionary<ushort, Type>>(fieldDictionary);
			// setPacketsDictionary = Delegator.createStaticFieldSetter<TwoWayDictionary<ushort, Type>>(fieldDictionary);
			getPacketsHighestIndex = Delegator.createStaticFieldGetter<ushort>(fieldIndex);
			setPacketsHighestIndex = Delegator.createStaticFieldSetter<ushort>(fieldIndex);
		}
		
		[Command(name: "ecc.lw.api.packets", Hidden = true)]
		public static void debugPacketIndices()
		{
			var dict = getPacketsDictionary();
			var index = getPacketsHighestIndex();
			LConsole.WriteLine("Listing all packets, highest index: " + index);
			foreach(var entry in dict.Forwards)
			{
				if(entry.Value == null)
				{
					LConsole.WriteLine(" - " + entry.Key + ": 'null'");
					continue;
				}
				LConsole.WriteLine(" - " + entry.Key + ": " + entry.Value.FullName);
			}
		}
		
		//As the field is readonly, conventional reflection has to be used (setValue()).
		// This is slower, but this is not meant to be called often anyway.
		public static void setPacketsDictionary(TwoWayDictionary<ushort, Type> value)
		{
			fieldDictionary.SetValue(null, value);
		}
		
		public static void removePacketsOfAssembly(Assembly assembly, out List<Type> removedPackets)
		{
			removedPackets = [];
			
			var currentHighestIndex = getPacketsHighestIndex();
			var currentDictionary = getPacketsDictionary();
			
			var index = currentHighestIndex;
			// First remove packets from the top of the packet list, this means decrementing the highest-index counter.
			while(index >= 1)
			{
				if(!checkAndRemovePacketAtIndex(index--, removedPackets))
				{
					break; // Break, as we just now did not remove from the top.
				}
			}
			
			// Update the highest index, if it was changed and packets got removed:
			if(index != (currentHighestIndex - 1))
			{
				//Increment again, to point to the next free index:
				setPacketsHighestIndex((ushort) (index + 1));
			}
			
			// Finally packets from the mod, that might be somewhere else in the packet mapping. So no highest-index modification.
			while(index >= 1)
			{
				checkAndRemovePacketAtIndex(index--, removedPackets);
			}
			
			bool checkAndRemovePacketAtIndex(ushort currentIndex, List<Type> removedPackets)
			{
				// Check if there is a packet with that ID:
				if(!currentDictionary.TryGetValue(currentIndex, out var packetType))
				{
					return false; // No packet here.
				}
				
				// Packet types should never be null, but be safe:
				if(packetType == null)
				{
					return false; // No packet type.
				}
				
				// Check if the type of the packet is part of the assembly to remove:
				if(packetType.Assembly != assembly)
				{
					return false; // Wrong packet type.
				}
				
				// Packet is part of the assembly to remove, thus remove it:
				currentDictionary.Remove(currentIndex, out var removedType); // Remove packet as requested.
				removedPackets.Add(removedType); // But remember it, so that the caller knows what to later re-inject to the packet list.
				return true;
			}
		}
	}
}
