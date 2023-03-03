using System;
using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Networking.Packets;
using MessagePack;

namespace WireTracer.Shared.Packets.S2C
{
	[MessagePackObject]
	public sealed class ClusterListingResponse : Packet
	{
		[Key(0)]
		public Guid requestGuid;
		[Key(1)]
		public List<ClusterDetails> selectedClusters;
		[Key(2)]
		public List<ClusterDetails> sourcingClusters;
		[Key(3)]
		public List<ClusterDetails> connectedClusters;
		[Key(4)]
		public List<ClusterDetails> drainingClusters;
	}

	[MessagePackObject]
	public sealed class ClusterDetails
	{
		[Key(0)]
		public List<PegAddress> pegs;
		[Key(1)]
		public List<ComponentAddress> connectingComponents;
		[Key(2)]
		public List<ComponentAddress> linkingComponents;
		
		//The following two entries are used on the client to temporary store wires until their outline is being removed again.
		// This information is just not collected on the server, hence the client collects them.
		[IgnoreMember]
		public List<WireAddress> highlightedWires;
		[IgnoreMember]
		public List<WireAddress> highlightedOutputWires;
	}
}
