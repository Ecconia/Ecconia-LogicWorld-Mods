using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Services;
using LogicWorld.Interfaces;
using LogicWorld.Outlines;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Client.Tool
{
	public class RemoteTracer : GenericTracer
	{
		private readonly ClusterListingResponse response;
		
		public RemoteTracer(ClusterListingResponse response)
		{
			this.response = response;
			// Populate/Collect wires (they are not sent by the server):
			foreach(var clusterDetails in response.selectedClusters)
			{
				populateWires(clusterDetails);
			}
			foreach(var clusterDetails in response.sourcingClusters)
			{
				populateWires(clusterDetails);
			}
			foreach(var clusterDetails in response.connectedClusters)
			{
				populateWires(clusterDetails);
			}
			foreach(var clusterDetails in response.drainingClusters)
			{
				populateWires(clusterDetails);
			}
			
			var world = Instances.MainWorld.Data;
			
			// Linking components are stored by (potentially) two connected clusters.
			// So a later drawn cluster can overwrite a custom peg outline with it's linking component.
			// To prevent this draw linking components before all pegs:
			drawLinkingComponents(world, response.selectedClusters);
			drawLinkingComponents(world, response.sourcingClusters);
			drawLinkingComponents(world, response.connectedClusters);
			drawLinkingComponents(world, response.drainingClusters);
			
			//Highlight primary cluster:
			drawClusters(world,
				response.selectedClusters,
				WireTracerColors.primaryConnected,
				WireTracerColors.primaryNormal
			);
			
			//Highlight source clusters:
			drawClusters(world,
				response.sourcingClusters,
				WireTracerColors.sourcingConnected,
				WireTracerColors.sourcingNormal
			);
			
			//Highlight connected clusters:
			drawClusters(world,
				response.connectedClusters,
				WireTracerColors.connectedConnected,
				WireTracerColors.connectedNormal
			);
			
			//Highlight draining clusters:
			drawClusters(world,
				response.drainingClusters,
				WireTracerColors.drainingConnected,
				WireTracerColors.drainingNormal
			);
		}
		
		private void drawLinkingComponents(IWorldData world, List<ClusterDetails> cluster)
		{
			foreach (var currentClusterDetails in cluster)
			{
				drawComponents(world, currentClusterDetails.linkingComponents, WireTracerColors.linking);
			}
		}
		
		private static void drawClusters(
			IWorldData world,
			List<ClusterDetails> clusters,
			OutlineData componentColor,
			OutlineData internalWireColor
		)
		{
			foreach(var currentClusterDetails in clusters)
			{
				drawComponents(world, currentClusterDetails.connectingComponents, componentColor);
				foreach(var address in currentClusterDetails.pegs)
				{
					// Skip things that do not exist (anymore/currently):
					if(world.Contains(address.ComponentAddress))
					{
						Outliner.Outline(address, address.IsInputAddress() ? internalWireColor : WireTracerColors.output);
					}
				}
				foreach(var address in currentClusterDetails.highlightedWires)
				{
					Outliner.Outline(address, internalWireColor);
				}
				foreach(var address in currentClusterDetails.highlightedOutputWires)
				{
					Outliner.Outline(address, WireTracerColors.output);
				}
			}
		}
		
		private static void drawComponents(IWorldData world, List<ComponentAddress> components, OutlineData componentColor)
		{
			foreach(var address in components)
			{
				// Skip things that do not exist (anymore/currently):
				if(world.Contains(address))
				{
					Outliner.Outline(address, componentColor);
				}
			}
		}
		
		/// <summary> Will collect all wires connected to the pegs of a cluster and cache them. Wires are sorted in internal and output-peg wires.</summary>
		private static void populateWires(ClusterDetails clusterDetails)
		{
			clusterDetails.highlightedWires = new List<WireAddress>();
			clusterDetails.highlightedOutputWires = new List<WireAddress>();
			foreach(var pegAddress in clusterDetails.pegs)
			{
				if(!pegAddress.IsInputAddress())
				{
					continue;
				}
				var wires = Instances.MainWorld.Data.LookupPegWires(pegAddress);
				if(wires == null)
				{
					continue;
				}
				foreach(var wireAddress in wires)
				{
					// At this point we skipped any output peg. The current address is an InputPeg.
					var wire = Instances.MainWorld.Data.Lookup(wireAddress);
					// We now check if the first point is the current InputPeg - as that way we outline each wire only once.
					// In cases where the first point is an OutputPeg, the second peg must be an InputPeg and we process this wire anyway.
					if(wire.Point1 == pegAddress || !wire.Point1.IsInputAddress())
					{
						// Sort the wire depending if it is connected to an OutputPeg or not. Output wires have a different color.
						(wire.Point1.IsInputAddress() && wire.Point2.IsInputAddress() ? clusterDetails.highlightedWires : clusterDetails.highlightedOutputWires).Add(wireAddress);
					}
				}
			}
		}
		
		public void stop()
		{
			foreach(var clusterDetails in response.selectedClusters)
			{
				unhighlightCluster(clusterDetails);
			}
			foreach(var clusterDetails in response.sourcingClusters)
			{
				unhighlightCluster(clusterDetails);
			}
			foreach(var clusterDetails in response.connectedClusters)
			{
				unhighlightCluster(clusterDetails);
			}
			foreach(var clusterDetails in response.drainingClusters)
			{
				unhighlightCluster(clusterDetails);
			}
		}
		
		private static void unhighlightCluster(ClusterDetails cluster)
		{
			foreach(var address in cluster.pegs)
			{
				Outliner.RemoveOutline(address);
			}
			foreach(var address in cluster.connectingComponents)
			{
				Outliner.RemoveOutline(address);
			}
			foreach(var address in cluster.linkingComponents)
			{
				Outliner.RemoveOutline(address);
			}
			foreach(var address in cluster.highlightedWires)
			{
				Outliner.RemoveOutline(address);
			}
			foreach(var address in cluster.highlightedOutputWires)
			{
				Outliner.RemoveOutline(address);
			}
		}
	}
}
