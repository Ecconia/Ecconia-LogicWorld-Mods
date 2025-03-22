using System;
using System.Collections.Generic;
using System.Linq;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicAPI.Services;
using LogicWorld.Server.Circuitry;
using WireTracer.Shared;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Server
{
	public static class ClusterCollector
	{
		//Reflection/Delegate access helpers:
		private static readonly Func<InputPeg, Cluster> getCluster;
		private static readonly Func<Cluster, ClusterLinker> getLinker;
		private static readonly Func<ClusterLinker, List<ClusterLinker>> getLeaders;
		private static readonly Func<ClusterLinker, List<ClusterLinker>> getFollowers;
		
		//Services needed to lookup wires/pegs:
		private static readonly ICircuitryManager circuits;
		private static readonly IWorldData world;
		
		static ClusterCollector()
		{
			getCluster = Delegator.createPropertyGetter<InputPeg, Cluster>(Properties.getPrivate(typeof(InputPeg), "Cluster"));
			getLinker = Delegator.createFieldGetter<Cluster, ClusterLinker>(Fields.getPrivate(typeof(Cluster), "Linker"));
			getLeaders = Delegator.createFieldGetter<ClusterLinker, List<ClusterLinker>>(Fields.getPrivate(typeof(ClusterLinker), "LinkedLeaders"));
			getFollowers = Delegator.createFieldGetter<ClusterLinker, List<ClusterLinker>>(Fields.getPrivate(typeof(ClusterLinker), "LinkedFollowers"));
			
			circuits = ServiceGetter.getService<ICircuitryManager>();
			world = ServiceGetter.getService<IWorldData>();
		}
		
		//### HELPER FUNCTIONS: ############
		
		private static bool doesPegExist(PegAddress address)
		{
			var component = world.Lookup(address.ComponentAddress);
			if(component == null)
			{
				return false; //Component of the peg does not exist in world.
			}
			var pegAmount = address.IsInputAddress() ? component.Data.InputCount : component.Data.OutputCount;
			//If false: Component does not have this peg, as the peg index is bigger than the actual components input/output count.
			return pegAmount >= address.PegIndex;
		}
		
		private static Cluster getClusterAt(InputAddress peg)
		{
			var originPeg = circuits.LookupInput(peg);
			if(originPeg == null)
			{
				throw new WireTracerException("Tried to lookup cluster on input peg, but the peg was not present in the circuit model! This should never happen, as the peg is present in the world.");
			}
			var cluster = getCluster(originPeg);
			if(cluster == null)
			{
				throw new WireTracerException("Tried to lookup cluster on input peg, but the cluster was 'null', this should never happen! As the peg is present in the world.");
			}
			return cluster;
		}
		
		private static bool getLinkerAt(Cluster cluster, out ClusterLinker linker)
		{
			linker = getLinker(cluster);
			return linker != null;
		}
		
		//### COLLECTION FUNCTIONS: ########
		
		public static bool collect(PegAddress originPegAddress, out ClusterListingResponse response)
		{
			//Validate, that the peg is actually existing:
			if(!doesPegExist(originPegAddress))
			{
				response = null;
				return false;
			}
			
			//An input peg, only has a single cluster.
			// An output peg however can be connected to multiple clusters.
			// It only makes sense to then select all these clusters as primary cluster.
			if(!collectMainClusters(originPegAddress, out var primaryClusters))
			{
				response = null;
				return false; //Whoops, cannot collect the primary clusters, probably probing an output peg.
			}
			
			//Collect clusters that get powered by the original clusters or will power it.
			var collectedSources = new HashSet<Cluster>();
			var collectedDrains = new HashSet<Cluster>();
			foreach(var cluster in primaryClusters)
			{
				collectLinkedClusters(cluster, collectedSources, getLeaders);
				collectLinkedClusters(cluster, collectedDrains, getFollowers);
			}
			foreach(var cluster in primaryClusters)
			{
				collectedSources.Remove(cluster);
				collectedDrains.Remove(cluster);
			}
			
			//Collect and filter clusters that are both source and drain:
			var collectedEquals = new HashSet<Cluster>();
			collectedSources.RemoveWhere(sourceCluster =>
			{
				if(!collectedDrains.Remove(sourceCluster))
				{
					collectedEquals.Add(sourceCluster);
					return true;
				}
				return false;
			});
			
			// Construct response and collect information about each cluster:
			response = new ClusterListingResponse
			{
				selectedClusters = primaryClusters.Select(collectClusterInformation).ToList(),
				sourcingClusters = collectedSources.Select(collectClusterInformation).ToList(),
				connectedClusters = collectedEquals.Select(collectClusterInformation).ToList(),
				drainingClusters = collectedDrains.Select(collectClusterInformation).ToList(),
			};
			return true;
		}
		
		/// <summary> Collects all clusters connected to a peg (OutputPegs have multiple clusters). </summary>
		private static bool collectMainClusters(PegAddress originPegAddress, out HashSet<Cluster> primaryClusters)
		{
			primaryClusters = new HashSet<Cluster>();
			if(originPegAddress.IsInputAddress(out var inputAddress))
			{
				// InputPegs only have a single cluster.
				primaryClusters.Add(getClusterAt(inputAddress));
			}
			else
			{
				// OutputPegs have multiple clusters, one per connected wire (duplication possible).
				var wires = world.LookupPegWires(originPegAddress);
				if(wires == null || wires.Count == 0)
				{
					//Well awkward situation, only this output peg will be highlighted.
					return false; //Refuse this request, as the client could figure this out itself.
				}
				foreach(var wireAddress in wires)
				{
					var wire = world.Lookup(wireAddress);
					if(wire == null)
					{
						throw new WireTracerException("Tried to lookup wire given its address, but the world did not contain it. World must be corrupted.");
					}
					// Get the other side of the wire - which is an InputPeg (not the OutputPeg):
					var otherSide = wire.Point1 == originPegAddress ? wire.Point2 : wire.Point1;
					if(!otherSide.IsInputAddress(out var otherSideInputAddress))
					{
						continue; //Not supported (output to output connection), this wire would be invalid anyway.
					}
					primaryClusters.Add(getClusterAt(otherSideInputAddress));
				}
			}
			return true;
		}
		
		/// <summary> Collect all linked clusters that are either follower or leader (based on provided link resolving method). </summary>
		private static void collectLinkedClusters(Cluster startingPoint, HashSet<Cluster> collectedClusters, Func<ClusterLinker, List<ClusterLinker>> linkedLinkerGetter)
		{
			var clustersToProcess = new Queue<ClusterLinker>();
			if(!getLinkerAt(startingPoint, out var startingLinker))
			{
				return; //No linker on this cluster => no link => nothing to collect
			}
			clustersToProcess.Enqueue(startingLinker);
			collectedClusters.Add(startingPoint); //While the starting cluster is no source, the algorithm needs to skip it when encountered.
			while(clustersToProcess.TryDequeue(out var linkerToCheck))
			{
				var listOfLinkedLinkers = linkedLinkerGetter(linkerToCheck); //Is never null.
				foreach(var linkedLinker in listOfLinkedLinkers)
				{
					var clusterOfLinkedLinker = linkedLinker.ClusterBeingLinked; //Should never be null.
					if(collectedClusters.Add(clusterOfLinkedLinker))
					{
						//Element was not yet present in the array, so keep looking into it!
						clustersToProcess.Enqueue(linkedLinker);
					}
				}
			}
		}
		
		/// <summary> Collects information about a cluster, which the client needs to outline it properly. </summary>
		private static ClusterDetails collectClusterInformation(Cluster cluster)
		{
			var details = new ClusterDetails
			{
				pegs = new List<PegAddress>(),
				connectingComponents = new List<ComponentAddress>(),
				linkingComponents = new List<ComponentAddress>(),
			};
			
			// These two lists are never null, according to how they are created and used:
			var inputPegs = cluster.ConnectedInputs;
			var outputPegs = cluster.ConnectedOutputs;
			
			foreach(var peg in inputPegs)
			{
				details.pegs.Add(peg.Address);
				
				// TBI: Why are these not collected into Sets? Does this not collect every ThroughPeg twice? Also what about modded components with secret and phasic links?
				// Highlight this component, when any of its pegs has a secret link attached:
				if(peg.SecretLinks != null && peg.SecretLinks.Any())
				{
					// Highlight this component somehow.
					details.connectingComponents.Add(peg.Address.ComponentAddress);
				}
				// Highlight this component, when any of its pegs has a phasic link attached:
				if(
					(peg.PhasicLinks != null && peg.PhasicLinks.Any())
					|| (peg.OneWayPhasicLinksFollowers != null && peg.OneWayPhasicLinksFollowers.Any())
					|| (peg.OneWayPhasicLinksLeaders != null && peg.OneWayPhasicLinksLeaders.Any())
				)
				{
					details.linkingComponents.Add(peg.Address.ComponentAddress);
				}
			}
			details.pegs.AddRange(outputPegs.Select(peg => peg.Address));
			return details;
		}
	}
}
