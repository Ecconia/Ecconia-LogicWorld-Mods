using System.Collections.Generic;
using System.Linq;
using CustomWirePlacer.Client.CWP.feature;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicUI;
using LogicWorld.Audio;
using LogicWorld.Building;
using LogicWorld.BuildingManagement;
using LogicWorld.GameStates;
using LogicWorld.Input;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public static class CustomWirePlacer
	{
		private static CWPGroup firstGroup = new CWPGroup();
		private static CWPGroup secondGroup = new CWPGroup();
		//The current group is used to reference the group which currently is being modified.
		private static CWPGroup currentGroup;

		//Indicates, if the mouse is down while editing a group. And not just down.
		private static bool drawing;
		//Normally always set when drawing is set, however sometimes drawing gets stopped before the mouse is up.
		// This flag here keeps the checking for mouse up going.
		private static bool applyOnUp;
		//When this is set, the next peg clicked will be used for the pattern of the second group.
		private static bool waitForPegToApplyPatternTo;

		private static bool pendingTwoDimensional;

		//Stores all generated and used wire-ghosts, to easily remove them again.
		private static readonly List<WireGhost> ghosts = new List<WireGhost>();

		public static bool pollStartWirePlacing()
		{
			if(!Trigger.DrawWire.DownThisFrame())
			{
				//Whatever draws a wire, must be done to actually draw a wire.
				// If that is not the case, rest of interaction may happen.
				return false;
			}
			PegAddress pegCurrentlyLookingAt = CWPHelper.getPegCurrentlyLookingAt();
			if(pegCurrentlyLookingAt == null)
			{
				//Not looking at a peg currently, so am most certainly not starting to drag a wire.
				// Continue with rest of interaction:
				return false;
			}
			//We are now starting to draw a wire:
			startWireDrawing(pegCurrentlyLookingAt);
			return true;
		}

		private static void startWireDrawing(PegAddress initialPeg)
		{
			bool isAlternativeMode = CWPTrigger.ModificatorAlternative.Held();
			if(isAlternativeMode)
			{
				PegDrawing.PegDrawing.switchToPegDrawingMode(initialPeg);
				return;
			}

			//Set the first peg:
			firstGroup.setFirstPeg(initialPeg);
			SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, CWPHelper.getWireConnectionPoint(initialPeg));

			//Switch state:
			GameStateManager.TransitionTo(CWPGameState.id);
		}

		public static void onActivate()
		{
			drawing = true; //Yes we are drawing!
			applyOnUp = true; //And on cursor up, we want to apply!
			waitForPegToApplyPatternTo = false;
			pendingTwoDimensional = false;

			currentGroup = firstGroup;

			//Handle settings:
			if(CWPSettings.resetFlipping)
			{
				//TODO: Actually make this setting worth it.
				CWPSettings.flipping = false;
			}

			//TODO: Enable again, but not now.
			// CWPStatusDisplay.setVisible(true);
		}

		private static void cleanUpWireGhosts()
		{
			foreach(WireGhost wire in ghosts)
			{
				wire.Recycle();
			}
			ghosts.Clear();
		}

		public static void onDeactivate()
		{
			CWPSettingsWindow.setVisible(false);
			CWPStatusDisplay.setVisible(false);

			//Undo all outlining, and reset all data:
			cleanUpWireGhosts();
			currentGroup = null;
			firstGroup.clear();
			secondGroup.clear();
		}

		public static void onUpdate()
		{
			if(UITrigger.Back.DownThisFrame() || Trigger.CancelPlacing.DownThisFrame())
			{
				//The user does not want to finish any wire placement action.
				currentGroup.clear(); //Whichever group we had been working on, it became invalid, it should not be overwritten. Else the Bus feature becomes a pain to use.
				// Done here, lets leave:
				SoundPlayer.PlayFail();
				GameStateManager.TransitionBackToBuildingState();
				return;
			}

			bool updated = false;

			if(CWPTrigger.GoTwoDimensional.DownThisFrame())
			{
				pendingTwoDimensional = !pendingTwoDimensional;
				if(currentGroup.isTwoDimensional())
				{
					pendingTwoDimensional = false;
				}
			}

			//Handle peg-selection and mouse-up while drawing:
			if(drawing)
			{
				PegAddress currentlyLookingAtPeg = CWPHelper.getPegCurrentlyLookingAt();
				if(currentlyLookingAtPeg != null)
				{
					if(currentlyLookingAtPeg == currentGroup.getStartPeg())
					{
						if(currentGroup.getSecondPeg() != null)
						{
							//Peg reset:
							currentGroup.setSecondPeg(null);
							if(secondGroup.isSet())
							{
								//In the first group, we are not making the sound, since then no wire is drawn at all.
								// But when a second group exists, the wire will update and a sound has to be played.
								SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, currentlyLookingAtPeg);
							}
							updated = true;
						}
					}
					else if(currentlyLookingAtPeg != currentGroup.getSecondPeg())
					{
						//Peg switched:
						currentGroup.setSecondPeg(currentlyLookingAtPeg);
						SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, currentlyLookingAtPeg);
						updated = true;
					}
				}

				if(CWPTrigger.ApplyPattern.DownThisFrame())
				{
					if(secondGroup.isSet())
					{
						//Currently drawing the second group. Apply the first group to it.
						secondGroup.applyGroup(firstGroup, secondGroup.getFirstPeg());
						updated = true;
						//Drawing must stop now, else the pattern may break.
						drawing = false;
					}
					else
					{
						SoundPlayer.PlayFail();
					}
				}
			}
			else //Handle the start of the second group and applying of build action.
			{
				if(CWPTrigger.ApplyPattern.DownThisFrame())
				{
					waitForPegToApplyPatternTo = true;
				}

				if(Trigger.DrawWire.DownThisFrame())
				{
					PegAddress lookingAt = CWPHelper.getPegCurrentlyLookingAt();
					if(lookingAt != null)
					{
						if(pendingTwoDimensional)
						{
							pendingTwoDimensional = false;
							//Starting two dimensional:
							applyOnUp = secondGroup.isSet();
							drawing = true;
							currentGroup.startTwoDimensional(lookingAt);
							updated = true;
						}
						else if(!secondGroup.isSet())
						{
							applyOnUp = true;
							if(waitForPegToApplyPatternTo)
							{
								waitForPegToApplyPatternTo = false;
								secondGroup.applyGroup(firstGroup, lookingAt);
							}
							else
							{
								//Starting to draw the second group!
								drawing = true;
								secondGroup.setFirstPeg(lookingAt);
							}
							currentGroup = secondGroup;
							updated = true;
						}
						else //Do the BUS feature action: (Second group must be set now)
						{
							applyNormalAction();
							(firstGroup, secondGroup) = (secondGroup, firstGroup);
							currentGroup = secondGroup;
							secondGroup.clear();
							updated = true;
							applyOnUp = true;
							if(waitForPegToApplyPatternTo)
							{
								waitForPegToApplyPatternTo = false;
								secondGroup.applyGroup(firstGroup, lookingAt);
							}
							else
							{
								drawing = true;
								secondGroup.setFirstPeg(lookingAt);
							}
						}
					}
				}
				//Else this click is for now meaningless.

				if(CWPTrigger.ApplyNormalAction.UpThisFrame())
				{
					if(updated)
					{
						//Same as above, due to return has to be updated here (if change).
						updateWireGhosts();
					}
					applyNormalAction();
					GameStateManager.TransitionBackToBuildingState(); //Does the cleanup.
					return;
				}
			}

			if(checkForMouseUp(updated))
			{
				return;
			}

			//Feature handling, that do not depend on the drawing state:

			//The expand/discover feature may actually be used while still drawing. However they get reset, if the second peg changes.
			if(CWPTrigger.ExpandBackwards.DownThisFrame())
			{
				currentGroup.expandBackwards();
				updated = true; //Always update, detecting changes is too complicated.
			}
			else if(CWPTrigger.ExpandFurther.DownThisFrame())
			{
				currentGroup.expandFurther();
				updated = true; //Always update, detecting changes is too complicated.
			}

			if(CWPTrigger.OpenSettings.DownThisFrame())
			{
				//TODO: When the settings window is closed, the mouse cursor may be lifted. Might need special handling.
				CWPSettingsWindow.toggleVisibility();
			}

			if(CWPTrigger.Flip.DownThisFrame())
			{
				CWPSettings.flipping = !CWPSettings.flipping;
				updated = true;
			}

			if(CWPTrigger.SkipModeSwitch.DownThisFrame())
			{
				//TODO: Make groups restore the mode, which was last used.
				currentGroup.switchSkipMode();
			}

			{
				bool up = Trigger.IncreaseMultiWirePlacingInterval.Held();
				bool down = Trigger.DecreaseMultiWirePlacingInterval.Held();
				if(up ^ down)
				{
					if(currentGroup.updateSkipNumber(up ? 1 : -1))
					{
						updated = true;
					}
					else
					{
						SoundPlayer.PlayFail();
					}
				}
			}

			if(CWPTrigger.RemoveWires.DownThisFrame())
			{
				if(secondGroup.isSet())
				{
					CWPRemoveWires.removeWiresFromGroups(firstGroup.getPegs().ToList(), secondGroup.getPegs().ToList());
				}
				else
				{
					CWPRemoveWires.removeWiresFromGroup(firstGroup.getPegs().ToList());
				}
				updated = true; //We might have deleted wires, that make room for the new ones.
				// While ofc, I doubt that the server response comes in the same frame.
			}

			if(updated)
			{
				updateWireGhosts();
			}
		}

		private static bool checkForMouseUp(bool updated)
		{
			if(Trigger.DrawWire.UpThisFrame())
			{
				drawing = false; //No longer drawing.
				if(!applyOnUp)
				{
					return false;
				}
				applyOnUp = false; //No longer handling mouse up.
				if(!CWPTrigger.Modificator.Held())
				{
					if(updated)
					{
						//Might have happened in this frame, but the update only happens at the end of a frame.
						// With the return here, it won't happen - and if the server takes some time, invalid wires will be shown.
						// Hence update it here.
						updateWireGhosts();
					}
					applyNormalAction();
					GameStateManager.TransitionBackToBuildingState(); //Does the cleanup.
					return true;
				}
				//Stall mode, here the group can be edited, or a new one started.
			}
			return false;
		}

		//TODO: Support peg skipping for in-line connections, but don't show wires, if there was a skip?

		private static void updateWireGhosts()
		{
			cleanUpWireGhosts();
			if(secondGroup.isSet())
			{
				List<PegAddress> smaller = firstGroup.getPegs().ToList();
				List<PegAddress> bigger = secondGroup.getPegs().ToList();
				if(CWPSettings.flipping)
				{
					bigger.Reverse(); //Not pretty, but does the job reliably.
				}
				if(smaller.Count > bigger.Count)
				{
					(smaller, bigger) = (bigger, smaller);
				}
				for(int i = 0; i < smaller.Count; i++)
				{
					connect(smaller[i], bigger[i], CWPOutlineData.validWire, CWPOutlineData.invalidWire);
				}
				PegAddress constant = smaller[smaller.Count - 1];
				for(int i = smaller.Count; i < bigger.Count; i++)
				{
					connect(constant, bigger[i], CWPOutlineData.validWire, CWPOutlineData.invalidWire);
				}
			}
			else if(!firstGroup.isTwoDimensional())
			{
				OutlineData valid = CWPOutlineData.validWire;
				OutlineData invalid = CWPOutlineData.invalidWire;
				if(firstGroup.hasExtraPegs())
				{
					valid = CWPOutlineData.validMultiWire;
					invalid = CWPOutlineData.invalidMultiWire;
				}
				//Drawing 1-group connections:
				IEnumerator<PegAddress> it = firstGroup.getAllPegs().GetEnumerator();
				it.MoveNext();
				PegAddress last = it.Current;
				while(it.MoveNext())
				{
					PegAddress current = it.Current;
					connect(last, current, valid, invalid);
					last = current;
				}
				it.Dispose();
			}

			void connect(PegAddress first, PegAddress second, OutlineData valid, OutlineData invalid)
			{
				if(first == second)
				{
					//Same peg, it is pointless to create this wire-ghost.
					return;
				}
				WireGhost wire = WireGhost.GetNewGhost();
				wire.SetInfo(first, second, 0f);
				wire.SetOutlineData(WireUtility.WireWouldBeValid(first, second) ? valid : invalid);
				ghosts.Add(wire);
			}
		}

		private static void applyNormalAction()
		{
			if(secondGroup.isSet())
			{
				//Two groups!
				List<BuildRequest> requests = new List<BuildRequest>();
				{
					List<PegAddress> smaller = firstGroup.getPegs().ToList();
					List<PegAddress> bigger = secondGroup.getPegs().ToList();
					if(CWPSettings.flipping)
					{
						bigger.Reverse(); //Not pretty, but does the job reliably.
					}
					if(smaller.Count > bigger.Count)
					{
						(smaller, bigger) = (bigger, smaller);
					}
					for(int i = 0; i < smaller.Count; i++)
					{
						PegAddress first = smaller[i];
						PegAddress second = bigger[i];
						if(first == second || !WireUtility.WireWouldBeValid(first, second))
						{
							continue;
						}
						requests.Add(new BuildRequest_CreateWire(new WireData(first, second, 0f)));
					}
					PegAddress constant = smaller[smaller.Count - 1];
					for(int i = smaller.Count; i < bigger.Count; i++)
					{
						PegAddress other = bigger[i];
						if(constant == other || !WireUtility.WireWouldBeValid(constant, other))
						{
							continue;
						}
						requests.Add(new BuildRequest_CreateWire(new WireData(constant, other, 0f)));
					}
				}
				if(requests.Any())
				{
					BuildRequestManager.SendManyBuildRequestsAsMultiUndoItem(requests);
				}
				else
				{
					SoundPlayer.PlayFail();
				}
			}
			else if(!firstGroup.isTwoDimensional())
			{
				//Only one group!
				//If the second peg is 'null' and no modifer was pressed, we only have a single peg. Hence just abort.
				if(firstGroup.getSecondPeg() != null)
				{
					//We have more than 1 peg.
					if(firstGroup.hasExtraPegs())
					{
						//We are placing more than 1 wire:
						List<BuildRequest> requests = new List<BuildRequest>();
						IEnumerator<PegAddress> it = firstGroup.getAllPegs().GetEnumerator();
						it.MoveNext();
						PegAddress last = it.Current;
						while(it.MoveNext())
						{
							PegAddress current = it.Current;
							if(WireUtility.WireWouldBeValid(last, current))
							{
								requests.Add(new BuildRequest_CreateWire(new WireData(last, current, 0f)));
							}
							last = current;
						}
						it.Dispose();
						if(requests.Any())
						{
							BuildRequestManager.SendManyBuildRequestsAsMultiUndoItem(requests);
						}
						else
						{
							SoundPlayer.PlayFail();
						}
					}
					else
					{
						//Only placing one wire:
						if(WireUtility.WireWouldBeValid(firstGroup.getFirstPeg(), firstGroup.getSecondPeg()))
						{
							BuildRequestManager.SendBuildRequest(new BuildRequest_CreateWire(new WireData(firstGroup.getFirstPeg(), firstGroup.getSecondPeg(), 0f)));
						}
						else
						{
							SoundPlayer.PlayFail();
						}
					}
				}
			}
		}
	}
}
