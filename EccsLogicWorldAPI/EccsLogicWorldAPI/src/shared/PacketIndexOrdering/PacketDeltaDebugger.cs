using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JimmysUnityUtilities.Collections;
using LICC;
using LogicAPI;

namespace EccsLogicWorldAPI.Shared.PacketIndexOrdering
{
	public static class PacketDeltaDebugger
	{
		private const bool ENABLE_DEBUG = false;
		
		private static List<MetaMod> metaMods = [];
		
		private static TwoWayDictionary<ushort, Type> lastPacketMap;
		private static int lastHighestIndex;
		
		public static void createInitial(List<MetaMod> metaMods)
		{
			if(!ENABLE_DEBUG)
			{
				return;
			}
			
			PacketDeltaDebugger.metaMods = metaMods;
			
			// Set initial values:
			lastPacketMap = PacketIndexHelper.getPacketsDictionary().Clone();
			lastHighestIndex = PacketIndexHelper.getPacketsHighestIndex();
			populateTypeAliases(lastPacketMap.Forwards.Values);
			
			// Print everything:
			var sb = new StringBuilder("Listing all packets:").AppendLine();
			
			var packets = lastPacketMap.Forwards.Select(pair => (pair.Key, pair.Value)).ToList();
			packets.Sort((a, b) => a.Key.CompareTo(b.Key));
			var lastAssembly = (Assembly) null;
			
			var notFirst = false;
			var lastIndex = 0;
			foreach(var (index, type) in packets)
			{
				lastIndex = index;
				
				if(type == null)
				{
					sb.Append(" - ").Append(index).Append(": null").AppendLine();
					lastAssembly = null;
				}
				else
				{
					if (lastAssembly == type.Assembly)
					{
						sb.Append("; ").Append(type.asText());
					}
					else
					{
						if (notFirst)
						{
							sb.Append(" >> ").Append(index - 1).AppendLine();
						}
						notFirst = true;
						sb.Append(" - ").Append(index).Append(": ").Append(type.asText());
					}
					lastAssembly = type.Assembly;
				}
			}
			sb.Append(" >> ").Append(lastIndex - 1).AppendLine();
			
			if (lastHighestIndex == lastIndex)
			{
				sb.Append("<#0f0>Highest index matches!");
			}
			else
			{
				sb.Append("<#f00>Highest index is wrong! Index is at: </color>").Append(lastHighestIndex);
			}
			
			LConsole.WriteLine(sb.ToString());
		}
		
		public static void printAndCreateDelta()
		{
			if(!ENABLE_DEBUG)
			{
				return;
			}
			
			// Get latest values:
			var packetMap = PacketIndexHelper.getPacketsDictionary().Clone();
			var highestIndex = PacketIndexHelper.getPacketsHighestIndex();
			populateTypeAliases(packetMap.Forwards.Values);
			
			// Find new/old packets, find index swaps:
			var removed = new List<(ushort index, Type type)>();
			var added = new List<(ushort index, Type type)>();
			var moved = new List<(ushort from, ushort to, Type type)>();
			foreach (var (index, type) in packetMap.Forwards)
			{
				if (lastPacketMap.Remove(type, out var oldIndex))
				{
					// Entry was present, check that the index was the same:
					if (index != oldIndex)
					{
						// Index was not the same, this means the packet was moved
						moved.Add((oldIndex, index, type));
					}
				}
				else
				{
					// Entry was not present, meaning it is new
					added.Add((index, type));
				}
			}
			// All remaining packets are removed in the new packet-map
			removed.AddRange(lastPacketMap.Forwards.Select(pair => (pair.Key, pair.Value)));
			
			// Now print the entries:
			var sb = new StringBuilder("Packet list changed:").AppendLine();
			
			removed.ForEach(entry => sb.Append("<#f55>REMOVED: </color>").Append(entry.type.asText()).Append(" @").Append(entry.index).AppendLine());
			added.ForEach(entry => sb.Append("<#8f8>ADDED: </color>").Append(entry.type.asText()).Append(" @").Append(entry.index).AppendLine());
			moved.ForEach(entry => sb.Append("<#ff8>Moved: </color>").Append(entry.type.asText()).Append(' ').Append(entry.from).Append(" -> ").Append(entry.to).AppendLine());
			
			if (highestIndex == packetMap.Forwards.Keys.Max())
			{
				sb.Append("<#0f0>Highest index matches!");
			}
			else
			{
				sb.Append("<#f00>Highest index is wrong! Index is at: </color>").Append(lastHighestIndex);
			}
			
			LConsole.WriteLine(sb);
			
			// Save the new as the old values:
			lastPacketMap = packetMap;
			lastHighestIndex = highestIndex;
		}
		
		// Name hackery:
		
		private static readonly Dictionary<Type, string> typeAliases = new Dictionary<Type, string>();
		
		private static string asText(this Type type)
		{
			return type == null ? "null" : typeAliases[type];
		}
		
		private static void populateTypeAliases(IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				if (typeAliases.ContainsKey(type))
				{
					continue;
				}
				
				var modID = type.Assembly == typeof(MetaMod).Assembly ? "MHG" : metaMods.Find(meta_i => meta_i.CodeAssembly == type.Assembly).Manifest.ID;
				var path = type.FullName!;
				var packetName = path[(path.LastIndexOf('.') + 1)..];
				typeAliases.Add(type, $"<#88f>{modID}</color>.<#f88>{packetName}</color>");
			}
		}
	}
}
