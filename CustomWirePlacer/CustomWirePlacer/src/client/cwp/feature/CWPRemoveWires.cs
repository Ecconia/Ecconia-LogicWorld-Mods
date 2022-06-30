using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicAPI.Services;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;

namespace CustomWirePlacer.Client.CWP.feature
{
	public static class CWPRemoveWires
	{
		public static void removeWiresFromGroup(List<PegAddress> pegs)
		{
			List<BuildRequest> requests = new List<BuildRequest>();
			IWorldData world = Instances.MainWorld.Data;
			for(int index = 0; index < pegs.Count; index++)
			{
				PegAddress peg = pegs[index];
				HashSet<WireAddress> wires = world.LookupPegWires(peg);
				if(wires == null)
				{
					continue;
				}
				foreach(WireAddress wireAddress in wires)
				{
					Wire wire = world.Lookup(wireAddress);
					PegAddress pegOtherSide = wire.Point1 == peg ? wire.Point2 : wire.Point1;
					//Sadly the pegs might be connected to each other, in that case we only want to delete the wire once.
					int otherIndex = pegs.IndexOf(pegOtherSide);
					if(otherIndex >= 0 //The partner peg is in the list of pegs to process!
						&& otherIndex <= index // we already processed this peg though.
					)
					{
						continue;
					}
					requests.Add(new BuildRequest_DeleteWire(wireAddress));
				}
			}
			if(requests.Any())
			{
				BuildRequestManager.SendManyBuildRequestsAsMultiUndoItem(requests);
			}
		}

		public static void removeWiresFromGroups(List<PegAddress> pegs, List<PegAddress> otherPegs)
		{
			List<BuildRequest> requests = new List<BuildRequest>();
			IWorldData world = Instances.MainWorld.Data;
			for(int index = 0; index < pegs.Count; index++)
			{
				PegAddress peg = pegs[index];
				HashSet<WireAddress> wires = world.LookupPegWires(peg);
				if(wires == null)
				{
					continue;
				}
				foreach(WireAddress wireAddress in wires)
				{
					Wire wire = world.Lookup(wireAddress);
					PegAddress pegOtherSide = wire.Point1 == peg ? wire.Point2 : wire.Point1;
					//Sadly the pegs might be connected to each other, in that case we only want to delete the wire once.
					int otherIndex = otherPegs.IndexOf(pegOtherSide);
					if(otherIndex >= 0) //The partner peg is in the other group of pegs, remove wire!
					{
						requests.Add(new BuildRequest_DeleteWire(wireAddress));
					}
				}
			}
			if(requests.Any())
			{
				BuildRequestManager.SendManyBuildRequestsAsMultiUndoItem(requests);
			}
		}
	}
}
