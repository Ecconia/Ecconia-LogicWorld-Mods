using System.Collections.Generic;
using LogicAPI.Data;
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
			//Populate wires:
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
			
			//Highlight primary cluster:
			foreach(var currentClusterDetails in response.selectedClusters)
			{
				foreach(var address in currentClusterDetails.connectingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.primaryConnected);
				}
				foreach(var address in currentClusterDetails.linkingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.linking);
				}
				foreach(var address in currentClusterDetails.pegs)
				{
					if(!world.Contains(address.ComponentAddress))
					{
						continue;
					}
					if(address.IsInputAddress())
					{
						Outliner.Outline(address, WireTracerColors.primaryNormal);
					}
					else
					{
						Outliner.Outline(address, WireTracerColors.primaryOutput);
					}
				}
				foreach(var address in currentClusterDetails.highlightedWires)
				{
					Outliner.Outline(address, WireTracerColors.primaryNormal);
				}
				foreach(var address in currentClusterDetails.highlightedOutputWires)
				{
					Outliner.Outline(address, WireTracerColors.primaryOutput);
				}
			}
			
			//Highlight source clusters:
			foreach(var currentClusterDetails in response.sourcingClusters)
			{
				foreach(var address in currentClusterDetails.connectingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.sourcingConnected);
				}
				foreach(var address in currentClusterDetails.linkingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.linking);
				}
				foreach(var address in currentClusterDetails.pegs)
				{
					if(!world.Contains(address.ComponentAddress))
					{
						continue;
					}
					if(address.IsInputAddress())
					{
						Outliner.Outline(address, WireTracerColors.sourcingNormal);
					}
					else
					{
						Outliner.Outline(address, WireTracerColors.sourcingOutput);
					}
				}
				foreach(var address in currentClusterDetails.highlightedWires)
				{
					Outliner.Outline(address, WireTracerColors.sourcingNormal);
				}
				foreach(var address in currentClusterDetails.highlightedOutputWires)
				{
					Outliner.Outline(address, WireTracerColors.sourcingOutput);
				}
			}
			
			//Highlight connected clusters:
			foreach(var currentClusterDetails in response.connectedClusters)
			{
				foreach(var address in currentClusterDetails.connectingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.connectedConnected);
				}
				foreach(var address in currentClusterDetails.linkingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.linking);
				}
				foreach(var address in currentClusterDetails.pegs)
				{
					if(!world.Contains(address.ComponentAddress))
					{
						continue;
					}
					if(address.IsInputAddress())
					{
						Outliner.Outline(address, WireTracerColors.connectedNormal);
					}
					else
					{
						Outliner.Outline(address, WireTracerColors.connectedOutput);
					}
				}
				foreach(var address in currentClusterDetails.highlightedWires)
				{
					Outliner.Outline(address, WireTracerColors.connectedNormal);
				}
				foreach(var address in currentClusterDetails.highlightedOutputWires)
				{
					Outliner.Outline(address, WireTracerColors.connectedOutput);
				}
			}
			
			//Highlight draining clusters:
			foreach(var currentClusterDetails in response.drainingClusters)
			{
				foreach(var address in currentClusterDetails.connectingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.drainingConnected);
				}
				foreach(var address in currentClusterDetails.linkingComponents)
				{
					if(!world.Contains(address))
					{
						continue;
					}
					Outliner.Outline(address, WireTracerColors.linking);
				}
				foreach(var address in currentClusterDetails.pegs)
				{
					if(!world.Contains(address.ComponentAddress))
					{
						continue;
					}
					if(address.IsInputAddress())
					{
						Outliner.Outline(address, WireTracerColors.drainingNormal);
					}
					else
					{
						Outliner.Outline(address, WireTracerColors.drainingOutput);
					}
				}
				foreach(var address in currentClusterDetails.highlightedWires)
				{
					Outliner.Outline(address, WireTracerColors.drainingNormal);
				}
				foreach(var address in currentClusterDetails.highlightedOutputWires)
				{
					Outliner.Outline(address, WireTracerColors.drainingOutput);
				}
			}
		}
		
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
					var wire = Instances.MainWorld.Data.Lookup(wireAddress);
					if(wire.Point1 == pegAddress || !wire.Point1.IsInputAddress()) //We do not collect wires from output pegs. So if the first is an output peg, the other side must be an input -> collect.
					{
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
