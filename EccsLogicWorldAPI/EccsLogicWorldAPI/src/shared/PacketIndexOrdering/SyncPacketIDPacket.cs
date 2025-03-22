using System.Collections.Generic;
using System.Text;
using LICC;
using LogicAPI.Networking.Packets;
using MessagePack;

namespace EccsLogicWorldAPI.Shared.PacketIndexOrdering
{
	[MessagePackObject]
	public class SyncPacketIDPacket : Packet
	{
		public const string FakeModName = "EccsLogicWorldAPI-RequestPacketIDSync";
		
		[Key(0)]
		public List<(string type, ushort index)> mandatoryPackets;
		[Key(1)]
		public List<(string type, ushort index)> optionalPackets;
		
		public SyncPacketIDPacket()
		{
			//This one is for MessagePack.
		}
		
		public void debugPrint()
		{
			var sb = new StringBuilder("Mandatory Packets:").AppendLine();
			foreach (var entry in mandatoryPackets)
			{
				sb.Append("- ").Append(entry.index).Append(": ").Append(entry.type).AppendLine();
			}
			sb.Append("Optional Packets:").AppendLine();
			foreach (var entry in optionalPackets)
			{
				sb.Append("- ").Append(entry.index).Append(": ").Append(entry.type).AppendLine();
			}
			LConsole.WriteLine(sb);
		}
	}
}
