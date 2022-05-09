using System;
using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;
using UnityEngine;

namespace CustomWirePlacer.Client.CWP
{
	public static class CWPHelper
	{
		public static Vector3 getWireConnectionPoint(PegAddress pegAddress)
		{
			return Instances.MainWorld.Renderer.Entities.GetWirePoint(pegAddress);
		}

		public static IEnumerable<PegAddress> collectPegsInBetween(PegAddress startPeg, PegAddress endPeg)
		{
			Vector3 start = getWireConnectionPoint(startPeg);
			Vector3 end = getWireConnectionPoint(endPeg);
			IEnumerable<PegAddress> collection =
				ChunkCaster
					.CastAll(start, end - start, Vector3.Distance(start, end), Masks.Peg)
					.OrderBy((Func<HitInfo, float>) (h => h.Hit.distance))
					.Select((Func<HitInfo, PegAddress>) (h => h.pAddress))
					.SkipLast(1);
			if(!collection.Any())
			{
				collection = null;
			}
			return collection;
		}

		public static PegAddress getPegCurrentlyLookingAt()
		{
			//Get collision with peg, however don't look through environment or structures.
			// TBI: It would be interesting to remove environment, if one then get draw wires like looking from below the world floor.
			return PlayerCaster.CameraCast(Masks.Environment | Masks.Structure | Masks.Peg).pAddress;
		}
	}
}
