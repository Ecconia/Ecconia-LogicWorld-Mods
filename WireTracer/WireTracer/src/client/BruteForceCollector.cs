using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Services;
using LogicWorld.Interfaces;

namespace WireTracer.Client
{
	//Get all contents of a cluster by "force" (iterating over the network graph [pegs & wires]). Not super efficient, it is preferred to use the server - but a good fallback.
	public static class BruteForceCollector
	{
		public static (HashSet<PegAddress>, List<(WireAddress, bool)>, List<ComponentAddress>) collect(WireAddress wireAddress)
		{
			//If grabbing a wire, always start from the next input peg.
			// Else it will select all wires from the output peg.
			Wire wire = Instances.MainWorld.Data.Lookup(wireAddress);
			return collect(wire.Point1.IsInput ? wire.Point1 : wire.Point2);
		}

		public static (HashSet<PegAddress>, List<(WireAddress, bool)>, List<ComponentAddress>) collect(PegAddress originPegAddress)
		{
			IWorldData world = Instances.MainWorld.Data;
			//Throws exception if that type does not exist (lets accept that):
			ComponentType throughPeg = Instances.MainWorld.ComponentTypes.GetComponentType("MHG.ThroughPeg");
			
			var collectedWires = new List<(WireAddress, bool)>();
			var collectedComponents = new List<ComponentAddress>();
			var collectedPegs = new HashSet<PegAddress>();
			collectedPegs.Add(originPegAddress); //Add the origin peg first. As if a wire leads to here, it is already processed.
			
			var pegs = new Queue<PegAddress>();
			pegs.Enqueue(originPegAddress); //Add the origin peg to the queue, to be processed first.
			
			while(pegs.Count != 0)
			{
				PegAddress thisSide = pegs.Dequeue(); //Unless this is the first peg, it should always be an input-peg.
				{
					IComponentInWorld component = world.Lookup(thisSide.ComponentAddress);
					if(component.Data.Type == throughPeg && component.Data.InputCount == 2) //Sanity check, confirm peg count.
					{
						collectedComponents.Add(thisSide.ComponentAddress);
						//This is a through peg, so lets also investigate the peg on the other side:
						PegAddress otherSide = new InputAddress(thisSide.ComponentAddress, thisSide.PegNumber == 0 ? 1 : 0); //Dirty but valid mapping
						if(collectedPegs.Add(otherSide))
						{
							//Other side was not yet processed!
							pegs.Enqueue(otherSide); //If we collect a new peg, we also have to check it.
						}
					}
				}
				var wireSet = world.LookupPegWires(thisSide);
				if(wireSet == null)
				{
					continue;
				}
				foreach(var wireAddress in wireSet)
				{
					Wire wire = world.Lookup(wireAddress);
					if(wire.Point1 == thisSide)
					{
						//Most easy way to prevent a wire from being added twice.
						// Pegs are in a HashSet, but wires only in a list.
						// But wires only have one first peg, when that is being processed add it.
						collectedWires.Add((wireAddress, !wire.Point1.IsInput || !wire.Point2.IsInput));
					}
					PegAddress otherSide = wire.Point1 == thisSide ? wire.Point2 : wire.Point1;
					if(collectedPegs.Add(otherSide))
					{
						//Other side was not yet processed!
						if(otherSide.IsInput)
						{
							pegs.Enqueue(otherSide); //If we collect a new peg, we also have to check it.
						}
						else //Output:
						{
							if(wire.Point1 == otherSide)
							{
								//If Point1 would have been 'thisSide', it would have been added earlier.
								// But if it is the 'otherSide', that means, it was not added and 'otherSide' will never be checked.
								// Hence it has to be added now.
								collectedWires.Add((wireAddress, true));
							}
						}
					}
					//Peg was already seen, meaning the wire was also already seen.
				}
			}
			return (collectedPegs, collectedWires, collectedComponents);
		}
	}
}
