using System.Collections.Generic;
using System.Linq;
using CustomWirePlacer.Client.CWP.feature;
using CustomWirePlacer.Client.Windows;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicUI;
using LogicWorld.Audio;
using LogicWorld.Building;
using LogicWorld.BuildingManagement;
using LogicWorld.GameStates;
using LogicWorld.Input;
using LogicWorld.Interfaces;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public static class CustomWirePlacer
	{
		private static CWPGroup firstGroup = new CWPGroup();
		private static CWPGroup secondGroup = new CWPGroup();
		//The current group is used to reference the group which currently is being modified.
		private static CWPGroup currentGroup;

		private static PegAddress lastLookedAtPeg;

		public static readonly CWPRaycastLine raycastLine = new CWPRaycastLine();

		private static bool active;
		//Indicates, if the mouse is down while editing a group. And not just down.
		private static bool drawing;
		//Normally always set when drawing is set, however sometimes drawing gets stopped before the mouse is up.
		// This flag here keeps the checking for mouse up going.
		private static bool applyOnUp;
		//When this is set, the next peg clicked will be used for the pattern of the second group.
		private static bool waitForPegToApplyPatternTo;

		private static bool doNotApplyExpandForward;
		private static bool doNotApplyExpandBackwards;

		private static bool pendingTwoDimensional;
		private static bool toggleListMode;

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

			// Set the first peg:
			firstGroup.setFirstPeg(initialPeg, false);
			raycastLine.setAxis(firstGroup.getCurrentAxis());
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
			doNotApplyExpandForward = doNotApplyExpandBackwards = false;
			toggleListMode = false;

			firstGroup.showFirstPeg();
			currentGroup = firstGroup;

			//Handle settings:
			CWPSettings.flipping = false;

			if(CWPSettings.showDetails)
			{
				CWPStatusDisplay.setVisible(true);
			}
			active = true;
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
			active = false;
			CWPSettingsWindow.setVisible(false);
			CWPStatusDisplay.setVisible(false);
			raycastLine.reset();

			//Undo all outlining, and reset all data:
			cleanUpWireGhosts();
			currentGroup = null;
			firstGroup.clear();
			secondGroup.clear();
			lastLookedAtPeg = null;
			CWPOutliner.RemoveAllOutlines(); //Cleanup leftover (baked) outlines.
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
					if(toggleListMode)
					{
						if(currentlyLookingAtPeg != lastLookedAtPeg)
						{
							currentGroup.toggleList(currentlyLookingAtPeg);
							updated = true;
						}
					}
					else if(currentlyLookingAtPeg == currentGroup.getFirstPeg())
					{
						if(currentGroup.getSecondPeg() != null)
						{
							//Peg reset:
							currentGroup.setSecondPeg(null);
							raycastLine.refresh();
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
						raycastLine.refresh();
						SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, currentlyLookingAtPeg);
						updated = true;
					}
				}

				if(CWPTrigger.ApplyPattern.DownThisFrame())
				{
					if(secondGroup.isSet())
					{
						//Currently drawing the second group. Apply the first group to it.
						secondGroup.applyGroup(firstGroup, secondGroup.getRootPeg());
						raycastLine.refresh();
						updated = true;
						//Drawing must stop now, else the pattern may break.
						drawing = false;
					}
					else
					{
						SoundPlayer.PlayFail();
					}
				}
				lastLookedAtPeg = currentlyLookingAtPeg;
			}
			else //Handle the start of the second group and applying of build action.
			{
				if(!CWPSettings.allowStartingWithOnePegGroup && !firstGroup.hasMultiplePegs())
				{
					//We only have one peg selected in the first group, yet that is not allowed by settings, abort.
					GameStateManager.TransitionBackToBuildingState();
					return;
				}
				
				if(CWPTrigger.ApplyPattern.DownThisFrame())
				{
					waitForPegToApplyPatternTo = true;
				}

				if(Trigger.DrawWire.DownThisFrame())
				{
					PegAddress lookingAt = CWPHelper.getPegCurrentlyLookingAt();
					if(lookingAt != null)
					{
						if(CWPTrigger.ModificatorAlternative.Held())
						{
							toggleListMode = true;
							drawing = true;
							currentGroup.toggleList(lookingAt);
							updated = true;
						}
						else if(pendingTwoDimensional)
						{
							pendingTwoDimensional = false;
							//Starting two dimensional:
							applyOnUp = secondGroup.isSet();
							drawing = true;
							currentGroup.bakePegOutlines();
							currentGroup.startTwoDimensional(lookingAt);
							updated = true;
							raycastLine.setAxis(currentGroup.getCurrentAxis());
						}
						else if(!secondGroup.isSet())
						{
							applyOnUp = true;
							firstGroup.bakePegOutlines();
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
							raycastLine.setAxis(currentGroup.getCurrentAxis());
						}
						else //Do the BUS feature action: (Second group must be set now)
						{
							applyNormalAction();
							(firstGroup, secondGroup) = (secondGroup, firstGroup);
							currentGroup = secondGroup;
							secondGroup.clear();
							CWPOutliner.RemoveAllOutlines();
							firstGroup.bakePegOutlines();
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
							raycastLine.setAxis(currentGroup.getCurrentAxis());
						}
					}
					lastLookedAtPeg = lookingAt;
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

			if(checkForMouseUp())
			{
				return;
			}

			//Feature handling, that do not depend on the drawing state:

			//The expand/discover feature may actually be used while still drawing. However they get reset, if the second peg changes.
			if(CWPTrigger.ExpandBackwards.UpThisFrame())
			{
				if(!doNotApplyExpandBackwards)
				{
					currentGroup.expandBackwards();
					raycastLine.refresh();
					updated = true; //Always update, detecting changes is too complicated.
				}
				doNotApplyExpandBackwards = false;
			}
			if(CWPTrigger.ExpandFurther.UpThisFrame())
			{
				if(!doNotApplyExpandForward)
				{
					currentGroup.expandFurther();
					raycastLine.refresh();
					updated = true; //Always update, detecting changes is too complicated.
				}
				doNotApplyExpandForward = false;
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
				updated = true;
			}

			{
				bool up = Trigger.IncreaseMultiWirePlacingInterval.Held();
				bool down = Trigger.DecreaseMultiWirePlacingInterval.Held();
				if(up ^ down)
				{
					int offset = up ? 1 : -1;
					bool doSkipping = true;
					if(CWPTrigger.ExpandBackwards.Held())
					{
						currentGroup.updateExpandBackwardsCount(offset);
						raycastLine.refresh();
						doNotApplyExpandBackwards = true;
						doSkipping = false;
						updated = true;
					}
					if(CWPTrigger.ExpandFurther.Held())
					{
						currentGroup.updateExpandFurtherCount(offset);
						raycastLine.refresh();
						doNotApplyExpandForward = true;
						doSkipping = false;
						updated = true;
					}
					if(doSkipping)
					{
						if(CWPTrigger.ModificatorAlternative.Held())
						{
							if(currentGroup.updateSkipOffset(offset))
							{
								updated = true;
							}
						}
						else if(currentGroup.updateSkipNumber(offset))
						{
							updated = true;
						}
						else
						{
							SoundPlayer.PlayFail();
						}
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
			
			raycastLine.onUpdate();
		}

		private static bool checkForMouseUp()
		{
			if(Trigger.DrawWire.UpThisFrame())
			{
				toggleListMode = false;
				drawing = false; //No longer drawing.
				if(!applyOnUp)
				{
					return false;
				}
				applyOnUp = false; //No longer handling mouse up.
				if(!CWPSettings.allowStartingWithOnePegGroup && !firstGroup.hasMultiplePegs())
				{
					//We only have one peg selected in the first group, yet that is not allowed by settings, abort.
					GameStateManager.TransitionBackToBuildingState();
					return true;
				}
				if(!CWPTrigger.Modificator.Held())
				{
					applyNormalAction();
					GameStateManager.TransitionBackToBuildingState(); //Does the cleanup.
					return true;
				}
				//Stall mode, here the group can be edited, or a new one started.
			}
			return false;
		}

		private static IEnumerable<(PegAddress first, PegAddress second, bool valid)> getWires()
		{
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
				var counts = getWireCounts(smaller, bigger);
				for(int i = 0; i < smaller.Count; i++)
				{
					PegAddress first = smaller[i];
					PegAddress second = bigger[i];
					if(shouldEmit(first, second))
					{
						yield return (first, second, isValid(first, second, counts));
					}
				}
				PegAddress constant = smaller[smaller.Count - 1];
				for(int i = smaller.Count; i < bigger.Count; i++)
				{
					PegAddress other = bigger[i];
					if(shouldEmit(constant, other))
					{
						yield return (constant, other, isValid(constant, other, counts));
					}
				}
			}
			else if(CWPSettings.connectPegsInOneGroupWithEachOther)
			{
				if(firstGroup.isTwoDimensional())
				{
					var offsets = firstGroup.get2DOffsets();
					foreach(var startingPeg in firstGroup.getFirstAxis().getPegs())
					{
						var pegs = CWPGroup.get2DPegs(startingPeg, offsets).ToList();
						//TBI: Literally the same code as in the block below, but the inputs are different. Not sure yet how to make this more pretty - while staying efficient enough. Could loop one more time...
						var counts = getWireCounts(pegs);
						PegAddress last = pegs[0];
						for(int i = 1; i < pegs.Count; i++)
						{
							PegAddress current = pegs[i];
							if(shouldEmit(last, current))
							{
								yield return (last, current, isValid(last, current, counts));
							}
							last = current;
						}
					}
				}
				else
				{
					var pegs = firstGroup.getPegs().ToList();
					var counts = getWireCounts(pegs);
					PegAddress last = pegs[0];
					for(int i = 1; i < pegs.Count; i++)
					{
						PegAddress current = pegs[i];
						if(shouldEmit(last, current))
						{
							yield return (last, current, isValid(last, current, counts));
						}
						last = current;
					}
				}
			}

			bool shouldEmit(PegAddress first, PegAddress second)
			{
				return first != second; //May happen, don't ever forward these.
			}

			bool isValid(PegAddress first, PegAddress second, Dictionary<PegAddress, int> counts)
			{
				//Before doing any ray casting, check that both pegs have at least one more slot for wires.
				if(counts[first] >= (first.IsInput ? WireUtility.MaxWiresPerInput : WireUtility.MaxWiresPerOutput))
				{
					return false;
				}
				if(counts[second] >= (second.IsInput ? WireUtility.MaxWiresPerInput : WireUtility.MaxWiresPerOutput))
				{
					return false;
				}
				//Both pegs can accept one more wire, check if the wire would be valid:
				bool valid = WireUtility.WireWouldBeValid(first, second);
				if(!valid)
				{
					//Not valid, don't add counts.
					return false;
				}
				//Both pegs have space for one more wire. So lets increase the wire count, since we potentially add another wire now:
				counts[first] += 1;
				counts[second] += 1;
				return true;
			}

			Dictionary<PegAddress, int> getWireCounts(List<PegAddress> pegs1, List<PegAddress> pegs2 = null)
			{
				var counts = new Dictionary<PegAddress, int>();
				foreach(var peg in pegs1)
				{
					var set = Instances.MainWorld.Data.LookupPegWires(peg);
					counts.TryAdd(peg, set == null ? 0 : set.Count);
				}
				if(pegs2 != null)
				{
					foreach(var peg in pegs2)
					{
						var set = Instances.MainWorld.Data.LookupPegWires(peg);
						counts.TryAdd(peg, set == null ? 0 : set.Count);
					}
				}
				return counts;
			}
		}

		public static void updateWireGhosts()
		{
			cleanUpWireGhosts();
			OutlineData validColor = CWPOutlineData.validWire;
			OutlineData invalidColor = CWPOutlineData.invalidWire;
			if(!secondGroup.isSet() && (firstGroup.isTwoDimensional() || firstGroup.hasExtraPegs()))
			{
				validColor = CWPOutlineData.validMultiWire;
				invalidColor = CWPOutlineData.invalidMultiWire;
			}
			foreach((PegAddress first, PegAddress second, bool valid) in getWires())
			{
				WireGhost wire = WireGhost.GetNewGhost();
				wire.SetInfo(first, second, 0f);
				wire.SetOutlineData(valid ? validColor : invalidColor);
				ghosts.Add(wire); //Add the wire to list, so that it can be removed on next update.
			}
		}

		private static void applyNormalAction()
		{
			cleanUpWireGhosts();
			List<BuildRequest> requests = new List<BuildRequest>();
			OutlineData outline = (!secondGroup.isSet() && (firstGroup.isTwoDimensional() || firstGroup.hasExtraPegs())) ? CWPOutlineData.validMultiWire : CWPOutlineData.validWire;
			foreach((PegAddress first, PegAddress second, bool valid) in getWires())
			{
				if(valid)
				{
					requests.Add(new BuildRequest_CreateWire(new WireData(first, second, 0f)));
					//Since I do not know which of the wire ghosts is related to which pegs,
					// there will just be new wire ghosts for only the valid wires:
					WireGhost wire = WireGhost.GetNewGhost();
					wire.SetInfo(first, second, 0f);
					wire.SetOutlineData(outline);
					wire.RecycleOnWorldUpdate();
				}
			}
			if(requests.Any())
			{
				if(requests.Count == 1)
				{
					BuildRequestManager.SendBuildRequest(requests[0]);
				}
				else
				{
					BuildRequestManager.SendManyBuildRequestsAsMultiUndoItem(requests);
				}
			}
			else
			{
				SoundPlayer.PlayFail();
			}
		}

		public static CWPGroup getCurrentGroup()
		{
			return currentGroup;
		}

		public static bool isActive()
		{
			return active;
		}
	}
}
