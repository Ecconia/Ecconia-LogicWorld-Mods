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

		public static IEnumerable<PegAddress> collectPegsInDirection(Vector3 position, Vector3 ray)
		{
			List<PegAddress> pegs = new List<PegAddress>();
			for(int i = 0; i < 10000; i++) //Okay this is the maximum that I can understand. Should never trigger though - well one never knows.
			{
				Vector3 probe = position + ray;
				Collider[] colliders = Physics.OverlapSphere(probe, 0.01f, Masks.Peg);
				if(colliders.Length != 1)
				{
					if(colliders.Length > 1)
					{
						ModClass.logger.Warn("Sphere-cast resulted in more than one peg. Why?");
					}
					return pegs.Any() ? pegs : null;
				}
				Collider collider = colliders[0];
				PegAddress address = Instances.MainWorld.Renderer.EntityColliders.GetPegAddress(collider);
				if(address == null)
				{
					ModClass.logger.Error("Casted for a peg, but got: " + collider.name + " : " + collider.tag);
				}
				pegs.Add(address);
				position = probe;
			}
			ModClass.logger.Error("Got stuck in peg-gathering loop, and exceeded 10000 iterations. Please report this issue.");
			return null;
		}
	}
}
