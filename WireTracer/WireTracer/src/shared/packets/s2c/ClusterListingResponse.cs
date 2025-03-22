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
		public List<ClusterDetails> selectedClusters; // Clusters which the player originally looked at.
		[Key(2)]
		public List<ClusterDetails> sourcingClusters; // Clusters which power the selected/primary clusters via phasic-links.
		[Key(3)]
		public List<ClusterDetails> connectedClusters; // Clusters which are connected via phasic-links bi-directionally to selected/primary clusters and thus are equal in power.
		[Key(4)]
		public List<ClusterDetails> drainingClusters; // Clusters which are getting powered by the selected/primary clusters via phasic-links.
	}
	
	[MessagePackObject]
	public sealed class ClusterDetails
	{
		[Key(0)]
		public List<PegAddress> pegs;
		[Key(1)]
		public List<ComponentAddress> connectingComponents; // Components like Sockets, which should be highlighted as they are an integral part of this clusters connectivity.
		[Key(2)]
		public List<ComponentAddress> linkingComponents; // Components which provide active phasic-links connecting the cluster to another. Warning: Both clusters share the same linking component. 
		
		//The following two entries are used on the client to temporary store wires until their outline is being removed again.
		// This information is just not collected on the server, hence the client collects them.
		[IgnoreMember]
		public List<WireAddress> highlightedWires;
		[IgnoreMember]
		public List<WireAddress> highlightedOutputWires;
	}
}
