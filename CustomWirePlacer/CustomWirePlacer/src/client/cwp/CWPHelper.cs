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

		public static PegAddress findNextPeg(Vector3 start, Vector3 ray)
		{
			if(Physics.Raycast(start, ray, out RaycastHit hit, 30, Masks.Peg))
			{
				return Instances.MainWorld.Renderer.EntityColliders.GetPegAddress(hit.collider);
			}
			return null;
		}

		public static PegAddress getPegRelativeToOtherPeg(PegAddress newPegOrigin, PegAddress oldPegOrigin, PegAddress oldPegPoint)
		{
			Vector3 oldOrigin = getWireConnectionPoint(oldPegOrigin);
			Vector3 oldPoint = getWireConnectionPoint(oldPegPoint);
			Vector3 offset = oldPoint - oldOrigin;
			Vector3 newOrigin = getWireConnectionPoint(newPegOrigin);
			Vector3 newPoint = newOrigin + offset;
			return getPegAt(newPoint);
		}

		public static PegAddress getPegAt(Vector3 position)
		{
			Collider[] colliders = Physics.OverlapSphere(position, 0.01f, Masks.Peg);
			if(colliders.Length != 1)
			{
				if(colliders.Length > 1)
				{
					ModClass.logger.Warn("Sphere-cast resulted in more than one peg. Why?");
				}
				return null;
			}
			Collider collider = colliders[0];
			PegAddress peg = Instances.MainWorld.Renderer.EntityColliders.GetPegAddress(collider);
			if(peg == null)
			{
				ModClass.logger.Error("Casted for a peg, but got: " + collider.name + " : " + collider.tag);
			}
			return peg;
		}
	}
}
