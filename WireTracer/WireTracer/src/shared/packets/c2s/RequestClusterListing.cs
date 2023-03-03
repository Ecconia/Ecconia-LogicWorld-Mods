using System;
using LogicAPI.Data;
using LogicAPI.Networking.Packets;
using MessagePack;

namespace WireTracer.Shared.Packets.C2S
{
	[MessagePackObject]
	public sealed class RequestClusterListing : Packet
	{
		[Key(0)]
		public Guid requestGuid;
		[Key(1)]
		public PegAddress pegAddress;
	}
}
