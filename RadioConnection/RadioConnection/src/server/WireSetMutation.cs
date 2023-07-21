using System.Collections.Generic;
using EccsLogicWorldAPI.Server;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Server.Components;
using LogicAPI.Services;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server.Managers;

namespace RadioConnection.Server
{
	public static class WireSetMutation
	{
		private static readonly IWorldData iWorldData;
		private static readonly IWorldUpdates iWorldUpdates;
		private static readonly IWorldDataMutator iWorldDataMutator;
		
		static WireSetMutation()
		{
			iWorldData = ServiceGetter.getService<IWorldData>();
			iWorldUpdates = ServiceGetter.getService<IWorldUpdates>();
			iWorldDataMutator = ServiceGetter.getService<IWorldDataMutator>();
		}
		
		private static void applyWorldMutation(WorldDataMutation mutation)
		{
			iWorldUpdates.QueueMutationToBeSentToClient(mutation);
			mutation.ApplyMutation(iWorldDataMutator);
		}
		
		public static void fixPegCount(ComponentAddress cAddress, int inputPegs)
		{
			new WorldMutation_ChangeDynamicComponentPegCounts()
			{
				AddressOfTargetComponent = cAddress,
				NewInputCount = inputPegs,
				NewOutputCount = 0,
			}.ApplyMutation(iWorldDataMutator);
		}
		
		public static void process(
			ComponentAddress cAddress,
			IReadOnlyList<IInputPeg> pegs,
			int oldListA,
			int oldListB,
			int newListA,
			int newListB
		)
		{
			var toRemove = new List<WireAddress>();
			var toPlace = new List<(WireAddress, float, PegAddress, PegAddress)>();
			if(newListA > oldListA)
			{
				//Have to move output wires up!
				var delta = newListA - oldListA;
				for(int i = oldListA; i < pegs.Count; i++)
				{
					var pegAddress = pegs[i].Address;
					foreach(var wireAddress in iWorldData.LookupPegWires(pegAddress).OrEmptyIfNull())
					{
						toRemove.Add(wireAddress);
						var wire = iWorldData.Lookup(wireAddress);
						var otherSide = wire.Point1 == pegAddress ? wire.Point2 : wire.Point1;
						toPlace.Add((wireAddress, wire.Rotation, new InputAddress(cAddress, i + delta), otherSide));
					}
				}
			}
			else if(newListA < oldListA)
			{
				//Move wires down to max peg:
				for(int i = newListA; i < oldListA; i++)
				{
					var pegAddress = pegs[i].Address;
					foreach(var wireAddress in iWorldData.LookupPegWires(pegAddress).OrEmptyIfNull())
					{
						toRemove.Add(wireAddress);
					}
				}
				//Have to move output wires down!
				var delta = newListA - oldListA;
				for(int i = oldListA; i < pegs.Count; i++)
				{
					var pegAddress = pegs[i].Address;
					foreach(var wireAddress in iWorldData.LookupPegWires(pegAddress).OrEmptyIfNull())
					{
						toRemove.Add(wireAddress);
						var wire = iWorldData.Lookup(wireAddress);
						var otherSide = wire.Point1 == pegAddress ? wire.Point2 : wire.Point1;
						toPlace.Add((wireAddress, wire.Rotation, new InputAddress(cAddress, i + delta), otherSide));
					}
				}
			}
			
			foreach(var wireAddress in toRemove)
			{
				applyWorldMutation(new WorldMutation_RemoveWire()
				{
					AddressOfWireToRemove = wireAddress,
				});
			}
			//Always raise a peg count update, even if it did not update.
			// As then LogicWorld will re-initialize the circuit network (and set the right pegs to ignore).
			// Minor diminishing overhead on some rare custom data update for more performance in simulation and less hackery.
			applyWorldMutation(new WorldMutation_ChangeDynamicComponentPegCounts()
			{
				AddressOfTargetComponent = cAddress,
				NewInputCount = newListA + newListB,
				NewOutputCount = 0,
			});
			foreach(var (wireAddress, rotation, a, b) in toPlace)
			{
				applyWorldMutation(new WorldMutation_AddNewWire()
				{
					AddressOfNewWire = wireAddress,
					DataOfNewWire = new WireData(a, b, rotation),
				});
			}
		}
	}
}
