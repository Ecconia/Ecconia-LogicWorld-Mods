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

		public static Vector3 getRaycastPoint(PegAddress pegAddress)
		{
			var entityManager = Instances.MainWorld.Renderer.Entities;
			if(CWPSettings.raycastAtBottomOfPegs)
			{
				Vector3 top = entityManager.GetWirePoint(pegAddress);
				var peg = entityManager.GetPegEntity(pegAddress);
				var bot = peg.WorldPosition;

				var wirePointDistance = (top - bot).magnitude;
				if(wirePointDistance < 0.025f)
				{
					//The bottom position will be at 0.025 height.
					// If the distance to the wire point is less, we rather take that.
					return top; //Beware of modders that supply weird peg heights.
				}
				//Default height of a peg is 0.3, a bit less than one third is required to handle most situations.
				// So lets choose something quite small.
				return bot + peg.up * 0.025f;
			}
			return entityManager.GetWirePoint(pegAddress);
		}

		public static IEnumerable<PegAddress> collectPegsInBetween(PegAddress startPeg, PegAddress endPeg)
		{
			Vector3 start = getRaycastPoint(startPeg);
			Vector3 end = getRaycastPoint(endPeg);
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

		//Gets the next peg on the ray, and returns the calculated center point of the peg along the ray.
		// This is done by ray-casting from the other side too.
		public static PegAddress findNextPeg(Vector3 start, Vector3 ray, out Vector3? position)
		{
			if(Physics.Raycast(start, ray, out RaycastHit hitEnter, 30, Masks.Peg))
			{
				var pegCollider = hitEnter.collider;
				var peg = Instances.MainWorld.Renderer.EntityColliders.GetPegAddress(pegCollider);
				var entryPoint = hitEnter.point;
				if(!pegCollider.Raycast(new Ray(entryPoint + ray * 3, -ray), out RaycastHit hitExit, 30))
				{
					ModClass.logger.Warn("FAILED to get the center of a peg, because second raycast did not hit... Weird. Using edge point.");
					position = entryPoint + ray * 0.04f; //Using entry point and some small offset, and pray that not too much breaks.
					return peg;
				}
				var exitPoint = hitExit.point;
				position = entryPoint + (exitPoint - entryPoint) / 2;
				return peg;
			}
			position = null;
			return null;
		}

		public static Vector3 getPegRayCenter(PegAddress peg, Vector3 start, Vector3 ray)
		{
			var collider = Instances.MainWorld.Renderer.Entities.GetPegEntity(peg).Collider;

			//I have to use a very high max distance, since the peg might be anywhere. No performance penalty here anyway.
			if(!collider.Raycast(new Ray(start, ray), out RaycastHit hitEntry, float.MaxValue))
			{
				throw new Exception("Ray does not hit Peg, while it should because it is in the list.");
			}
			var pointEntry = hitEntry.point;
			if(!collider.Raycast(new Ray(pointEntry + ray * 3, -ray), out RaycastHit hitExit, float.MaxValue))
			{
				throw new Exception("Ray does not hit Peg, while it should because it is in the list and just already got hit forwards.");
			}
			var pointExit = hitExit.point;
			return pointEntry + (pointExit - pointEntry) / 2;
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
