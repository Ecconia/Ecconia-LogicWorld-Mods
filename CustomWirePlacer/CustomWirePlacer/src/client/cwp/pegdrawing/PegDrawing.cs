using System.Collections.Generic;
using System.Linq;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicUI;
using LogicWorld.Audio;
using LogicWorld.Building;
using LogicWorld.Building.Overhaul;
using LogicWorld.BuildingManagement;
using LogicWorld.GameStates;
using LogicWorld.Input;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Data;
using UnityEngine;

namespace CustomWirePlacer.Client.CWP.PegDrawing
{
	public static class PegDrawing
	{
		private static readonly HotbarItemData pegData = new BasicHotbarItemData("MHG.Peg");

		private static PegAddress peg;

		private static StuffPlacer placer;
		private static WireGhost wire;
		private static Collider ghostlyCollider;

		public static void switchToPegDrawingMode(PegAddress initialPeg)
		{
			if(isPegFull(initialPeg))
			{
				SoundPlayer.PlayFail();
				return;
			}
			peg = initialPeg;
			GameStateManager.TransitionTo(GameStatePegDrawing.id);
		}

		private static bool isPegFull(PegAddress pegAddress)
		{
			HashSet<WireAddress> wireAddressSet = Instances.MainWorld.Data.LookupPegWires(pegAddress);
			int amountOfWires = wireAddressSet == null ? 0 : wireAddressSet.Count;
			int maxAmountOfWires = pegAddress.IsInput ? WireUtility.MaxWiresPerInput : WireUtility.MaxWiresPerOutput;
			return amountOfWires >= maxAmountOfWires;
		}

		public static void onActivate()
		{
			//Peg initialization:
			PlacingGhost ghost = PlacingGhost.CreateForHotbarItem(pegData);
			placer = new StuffPlacer(ghost, ghost.GhostWorld.Dynamics.GetPlacingRulesAt(ghost.RootComponent))
			{
				AllowOffsetHold = true,
			};
			//Wire initialization:
			wire = WireGhost.GetNewGhost();
			wire.GameObject.SetActive(false);
			ghostlyCollider = ghost.Colliders.BoxColliders.First();
			wire.SetInfo(ghostlyCollider, peg, 0f);
		}

		public static void onDeactivate()
		{
			peg = null;

			//Peg:
			placer.Ghost.Delete();
			placer = null;
			//Wire:
			wire.GameObject.SetActive(true); //Better safe, than corrupted instances.
			wire.Recycle();
			wire = null;
			ghostlyCollider = null;
		}

		public static void onUpdate()
		{
			if(UITrigger.Back.DownThisFrame() || Trigger.CancelPlacing.DownThisFrame())
			{
				SoundPlayer.PlayFail();
				GameStateManager.TransitionBackToBuildingState();
				return;
			}

			//Ghost peg placer:
			placer.SyncCastingDataToPlayerCamera();
			placer.RunStuffPlacing();

			//Ghost wire:
			PlacingGhost ghost = placer.Ghost;
			wire.GameObject.SetActive(!ghost.IsHidden); //Will 
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
					apply(ghost, wire.Peg1.WorldspacePoint, peg);
				}
				else
				{
					SoundPlayer.PlayFail();
				}
				GameStateManager.TransitionBackToBuildingState(); //Regardless stop CWP here.
			}
		}

		private static void apply(PlacingGhost ghost, Vector3 ghostTargetPos, PegAddress mainPeg)
		{
			IEditableComponentData data = ghost.GhostWorld.Data.Lookup(ghost.RootComponent).Data;
			data.Parent = ghost.PreviousMoveAddress;
			data.LocalPosition = ghost.GetLocalPosition();
			data.LocalRotation = ghost.GetLocalRotation();
			BuildRequestManager.SendBuildRequest(new BuildRequest_CreateSingleNewComponent((ComponentData) data), receipt =>
			{
				if(receipt.ActionSuccessfullyApplied)
				{
					//The confirmation is there, however the peg will not be ready until the next frame. So wait until then:
					CoroutineUtility.RunAfterOneFrame(() =>
					{
						if(attemptWirePlacement(ghostTargetPos, mainPeg))
						{
							ModClass.logger.Warn("Failed to find the peg that just got placed by server, for the Peg-Drawing feature. Please report this issue to the developer.");
						}
					});
				}
			});
		}

		private static bool attemptWirePlacement(Vector3 fromPosition, PegAddress toPeg)
		{
			PegAddress newlyPlacedPeg = CWPHelper.getPegAt(fromPosition);
			if(newlyPlacedPeg != null && WireUtility.WireWouldBeValid(toPeg, newlyPlacedPeg))
			{
				BuildRequestManager.SendBuildRequest(new BuildRequest_CreateWire(new WireData(toPeg, newlyPlacedPeg, 0f)));
				return false;
			}
			return true;
		}
	}
}
