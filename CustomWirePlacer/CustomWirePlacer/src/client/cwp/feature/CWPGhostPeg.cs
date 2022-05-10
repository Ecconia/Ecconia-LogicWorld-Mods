using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.Audio;
using LogicWorld.Building;
using LogicWorld.Building.NewShit;
using LogicWorld.BuildingManagement;
using LogicWorld.GameStates;
using LogicWorld.Input;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Data;
using LogicWorld.SharedCode.Components;
using UnityEngine;
using Object = System.Object;

namespace CustomWirePlacer.Client.CWP.feature
{
	public static class CWPGhostPeg
	{
		private static bool active;
		private static readonly HotbarItemData pegData = new BasicHotbarItemData("MHG.Peg");
		private static StuffPlacer placer;
		private static WireGhost wire;
		private static Collider ghostlyCollider;

		public static void update(bool keepGhostPegAlive)
		{
			if(keepGhostPegAlive != active)
			{
				active = keepGhostPegAlive;
				if(keepGhostPegAlive) //Boot:
				{
					PegAddress mainPeg = CustomWirePlacer.firstGroup.getFirstPeg();
					if(isPegFull(mainPeg))
					{
						active = false;
						return; //No execution, wire can't be placed.
					}

					//Peg:
					PlacingGhost ghost = PlacingGhost.CreateForHotbarItem(pegData);
					PlacingRules placingRulesAt = ghost.GhostWorld.Dynamics.GetPlacingRulesAt(ghost.RootComponent);
					placer = new StuffPlacer(ghost, placingRulesAt);
					placer.AllowOffsetHold = true;
					//Wire:
					wire = WireGhost.GetNewGhost();
					ghostlyCollider = ghost.Colliders.BoxColliders.First();
					wire.SetInfo(ghostlyCollider, mainPeg, 0f);
				}
				else //Shutdown:
				{
					//Peg:
					placer.Ghost.Delete();
					placer = null;
					//Wire:
					wire.Recycle();
					wire = null;
					ghostlyCollider = null;
				}
			}

			if(active)
			{
				//Ghost peg placer:
				placer.SyncCastingDataToPlayerCamera();
				placer.RunStuffPlacing();

				//Ghost wire:
				PlacingGhost ghost = placer.Ghost;
				Vector3 up = ghost.Transform.up;
				wire.Peg1 = new PegRenderData()
				{
					WorldspaceComponentUp = up,
					WorldspaceUp = up,
					WorldspacePoint = ghost.Transform.position + up * 0.3f * 0.9f,
					IsInput = true,
				};
				wire.ReDraw();
				ghostlyCollider.enabled = true;
				wire.UpdateValidityAndOutline();
				ghostlyCollider.enabled = false;

				if(Trigger.DrawWire.UpThisFrame())
				{
					if(placer.CanPlaceGhost() && wire.ValidOnLastCheck)
					{
						//Apply the build-action!
						apply(ghost, wire.Peg1.WorldspacePoint, CustomWirePlacer.firstGroup.getFirstPeg());
					}
					else
					{
						SoundPlayer.PlayFail();
					}
					GameStateManager.TransitionBackToBuildingState(); //Regardless stop CWP here.
				}
			}
		}

		private static bool isPegFull(PegAddress pegAddress)
		{
			HashSet<WireAddress> wireAddressSet = Instances.MainWorld.Data.LookupPegWires(pegAddress);
			int amountOfWires = wireAddressSet == null ? 0 : wireAddressSet.Count;
			return amountOfWires >= WireUtility.MaxWiresPerInput;
		}

		private static Action a;

		public static void apply(PlacingGhost ghost, Vector3 ghostTargetPos, PegAddress mainPeg)
		{
			IEditableComponentData data = ghost.GhostWorld.Data.Lookup(ghost.RootComponent).Data;
			data.Parent = ghost.PreviousMoveAddress;
			data.LocalPosition = ghost.GetLocalPosition();
			data.LocalRotation = ghost.GetLocalRotation();

			BuildRequestManager.SendBuildRequest(new BuildRequest_CreateSingleNewComponent((ComponentData) data), receipt =>
			{
				if(receipt.ActionSuccessfullyApplied)
				{
					//TODO: This code is so major ugly. Idk. It does work however... But I hope this feature is worth it. To be fair, this whole class is suspicious.
					int loops = 0;
					a = () =>
					{
						if(loops++ > 10)
						{
							ModClass.logger.Warn("Was not able to cast placed ghost peg, attempted about 10 times every 30ms, RIP.");
							return;
						}
						PegAddress newlyPlacedPeg = CWPHelper.getPegAt(ghostTargetPos);
						if(newlyPlacedPeg != null && WireUtility.WireWouldBeValid(mainPeg, newlyPlacedPeg))
						{
							BuildRequestManager.SendBuildRequest(new BuildRequest_CreateWire(new WireData(mainPeg, newlyPlacedPeg, 0f)));
						}
						else
						{
							new Timer((Object _) => { JimmysUnityUtilities.Dispatcher.Invoke(a); }, new AutoResetEvent(false), 30, Timeout.Infinite);
						}
					};
					JimmysUnityUtilities.Dispatcher.Invoke(a);
				}
			});
		}
	}
}
