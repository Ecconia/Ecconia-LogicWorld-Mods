using System.Collections.Generic;
using System.Linq;
using CustomWirePlacer.Client.CWP.feature;
using CustomWirePlacer.Client.Windows;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicUI;
using LogicUI.MenuTypes;
using LogicWorld.Audio;
using LogicWorld.Building;
using LogicWorld.BuildingManagement;
using LogicWorld.GameStates;
using LogicWorld.Input;
using LogicWorld.Interfaces;
using LogicWorld.Outlines;
using LogicWorld.UI.HelpList;

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
		public static bool waitForPegToApplyPatternTo;
		public static bool flipping;

		private static bool enteredStallMode;

		public static bool doNotApplyExpandForward;
		public static bool doNotApplyExpandBackwards;

		public static bool pendingTwoDimensional;
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
			if(CWPTrigger.ModificatorAlternative.Held())
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
			flipping = false;
			enteredStallMode = false;

			firstGroup.showFirstPeg();
			currentGroup = firstGroup;

			if(CWPSettings.showStatusOverlay)
			{
				CWPStatusOverlay.setVisible(true);
			}
			active = true;
			CWPHelpOverlay.updateText(); //Has to be called manually, because it only works after this initialization.
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
			CWPStatusOverlay.setVisible(false);
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

			if(Trigger.ToggleHelp.DownThisFrame())
			{
				ToggleableSingletonMenu<HelpListMenu>.ToggleMenu();
			}

			bool updated = false;

			if(CWPTrigger.GoTwoDimensional.DownThisFrame())
			{
				bool old = pendingTwoDimensional;
				pendingTwoDimensional = !pendingTwoDimensional;
				if(currentGroup.isTwoDimensional())
				{
					pendingTwoDimensional = false;
				}
				if(pendingTwoDimensional != old)
				{
					CWPStatusOverlay.setDirtyGeneric();
					CWPHelpOverlay.updateText();
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
							//Group has only one peg now -> Update the help, no longer stall mode with first group, and expand disabled.
							CWPHelpOverlay.updateText();
							updated = true;
						}
					}
					else if(currentlyLookingAtPeg != currentGroup.getSecondPeg())
					{
						bool updateHelp = currentGroup.getCurrentAxis().secondPeg == null || !currentGroup.hasMultiplePegs();
						//Peg switched:
						currentGroup.setSecondPeg(currentlyLookingAtPeg);
						if(updateHelp)
						{
							//More than one peg in first group -> Update the help, because we might no longer be able to go to stall mode.
							//Or had no second peg, but now have a second peg, expand in help needs to be updated.
							CWPHelpOverlay.updateText();
						}
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
						if(secondGroup.applyGroup(firstGroup, secondGroup.getRootPeg()))
						{
							raycastLine.refresh();
							updated = true;
							//Drawing must stop now, else the pattern may break.
							drawing = false;
						}
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
					waitForPegToApplyPatternTo = !waitForPegToApplyPatternTo;
					CWPStatusOverlay.setDirtyGeneric();
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
								if(!secondGroup.applyGroup(firstGroup, lookingAt))
								{
									//Fallback to only setting the first peg. It is too late to undo everything.
									secondGroup.setFirstPeg(lookingAt);
								}
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
								if(!secondGroup.applyGroup(firstGroup, lookingAt))
								{
									//Fallback to only setting the first peg. It is too late to undo everything.
									secondGroup.setFirstPeg(lookingAt);
								}
							}
							else
							{
								drawing = true;
								secondGroup.setFirstPeg(lookingAt);
							}
							raycastLine.setAxis(currentGroup.getCurrentAxis());
						}
						//The mouse click state changed, update the help text, because there very likely was a change to the state.
						CWPHelpOverlay.updateText();
					}
					lastLookedAtPeg = lookingAt;
				}
				//Else this click is for now meaningless.

				if(CWPTrigger.ApplyNormalAction.UpThisFrame())
				{
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
			if(CWPTrigger.ExpandFurther.DownThisFrame() || CWPTrigger.ExpandBackwards.DownThisFrame())
			{
				CWPHelpOverlay.updateText();
			}
			if(CWPTrigger.ExpandBackwards.UpThisFrame())
			{
				if(!doNotApplyExpandBackwards)
				{
					currentGroup.expandBackwards();
					raycastLine.refresh();
					updated = true; //Always update, detecting changes is too complicated.
				}
				doNotApplyExpandBackwards = false;
				CWPHelpOverlay.updateText();
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
				CWPHelpOverlay.updateText();
			}

			if(CWPTrigger.OpenSettings.DownThisFrame())
			{
				CWPSettingsWindow.toggleVisibility();
			}

			if(CWPTrigger.Flip.DownThisFrame())
			{
				flipping = !flipping;
				updated = true;
			}

			if(CWPTrigger.SkipModeSwitch.DownThisFrame())
			{
				//TODO: Make groups restore the mode, which was last used.
				currentGroup.switchSkipMode();
				updated = true;
			}

			if(CWPTrigger.ModificatorAlternative.UpThisFrame() || CWPTrigger.ModificatorAlternative.DownThisFrame())
			{
				//Required, to update the skip text.
				CWPHelpOverlay.updateText();
			}
			{
				bool up = CWPTrigger.IncreaseInterval.Held();
				bool down = CWPTrigger.DecreaseInterval.Held();
				if(up ^ down)
				{
					int offset = up ? 1 : -1;
					bool doSkipping = true;
					if(CWPTrigger.ExpandBackwards.Held())
					{
						currentGroup.updateExpandBackwardsCount(offset);
						raycastLine.refresh();
						bool old = doNotApplyExpandBackwards;
						doNotApplyExpandBackwards = true;
						if(doNotApplyExpandBackwards != old)
						{
							CWPHelpOverlay.updateText();
						}
						doSkipping = false;
						updated = true;
					}
					if(CWPTrigger.ExpandFurther.Held())
					{
						currentGroup.updateExpandFurtherCount(offset);
						raycastLine.refresh();
						bool old = doNotApplyExpandForward;
						doNotApplyExpandForward = true;
						if(doNotApplyExpandForward != old)
						{
							CWPHelpOverlay.updateText();
						}
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

			if(!updated && !CWPSettings.connectPegsInOneGroupWithEachOther && (CWPTrigger.Modificator.UpThisFrame() || CWPTrigger.Modificator.DownThisFrame()))
			{
				updated = true; //When not connecting pegs in one group, the mod key might cause a visual update. Hence update when it is toggled.
			}

			if(updated)
			{
				updateWireGhosts();
				CWPStatusOverlay.setDirtyGeneric();
			}

			raycastLine.onUpdate();
		}

		private static bool checkForMouseUp()
		{
			if(!drawing || Trigger.DrawWire.Held())
			{
				//If not drawing or still holding draw key, abort.
				return false;
			}
			//Else we have to turn off the drawing mode:
			toggleListMode = false;
			drawing = false;
			if(!applyOnUp)
			{
				//We ran an action that stopped drawing mode, before the mouse was released.
				CWPHelpOverlay.updateText(); //Switching state (going into stall), update help.
				enteredStallMode = true;
				return false;
			}
			applyOnUp = false; //No longer handling mouse up.
			if(!CWPSettings.allowStartingWithOnePegGroup && !firstGroup.hasMultiplePegs() && !secondGroup.isSet())
			{
				//We only have one peg selected in the first group, yet that is not allowed by settings, abort.
				GameStateManager.TransitionBackToBuildingState();
				return true;
			}
			if(CWPTrigger.Modificator.Held() || !Trigger.DrawWire.UpThisFrame())
			{
				//Either we are currently holding MOD, which goes into stall mode.
				//Or the mouse was released while CWP did not have the focus, then it also switches to stall mode to be safe.
				CWPHelpOverlay.updateText(); //Switching state (going into stall), update help.
				enteredStallMode = true;
				return false;
			}
			//Apply the normal actions, nothing prevents it:
			applyNormalAction();
			GameStateManager.TransitionBackToBuildingState(); //Does the cleanup.
			return true;
		}

		private static IEnumerable<(PegAddress first, PegAddress second, bool valid)> getWires()
		{
			if(secondGroup.isSet())
			{
				List<PegAddress> smaller = firstGroup.getPegs();
				List<PegAddress> bigger = secondGroup.getPegs();
				NoDuplicationHasher<PegAddress> noDuplicationHasher = new NoDuplicationHasher<PegAddress>((uint) (smaller.Count + bigger.Count));
				if(flipping)
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
					if(shouldEmit(first, second) && noDuplicationHasher.probeDuplicate(first, second))
					{
						yield return (first, second, isValid(first, second, counts));
					}
				}
				PegAddress constant = smaller[smaller.Count - 1];
				for(int i = smaller.Count; i < bigger.Count; i++)
				{
					PegAddress other = bigger[i];
					if(shouldEmit(constant, other) && noDuplicationHasher.probeDuplicate(constant, other))
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
					var startingPegs = firstGroup.getFirstAxis().getPegs();
					NoDuplicationHasher<PegAddress> noDuplicationHasher = new NoDuplicationHasher<PegAddress>((uint) (offsets.Count * startingPegs.Count));
					foreach(var startingPeg in startingPegs)
					{
						var pegs = CWPGroup.get2DPegs(startingPeg, offsets).ToList();
						//TBI: Literally the same code as in the block below, but the inputs are different. Not sure yet how to make this more pretty - while staying efficient enough. Could loop one more time...
						var counts = getWireCounts(pegs);
						PegAddress last = pegs[0];
						for(int i = 1; i < pegs.Count; i++)
						{
							PegAddress current = pegs[i];
							if(shouldEmit(last, current) && noDuplicationHasher.probeDuplicate(last, current))
							{
								yield return (last, current, isValid(last, current, counts));
							}
							last = current;
						}
					}
				}
				else
				{
					var pegs = firstGroup.getPegs();
					NoDuplicationHasher<PegAddress> noDuplicationHasher = new NoDuplicationHasher<PegAddress>((uint) pegs.Count);
					var counts = getWireCounts(pegs);
					PegAddress last = pegs[0];
					for(int i = 1; i < pegs.Count; i++)
					{
						PegAddress current = pegs[i];
						if(shouldEmit(last, current) && noDuplicationHasher.probeDuplicate(last, current))
						{
							yield return (last, current, isValid(last, current, counts));
						}
						last = current;
					}
				}
			}
			else
			{
				firstGroup.getPegs(); //Literally just call this to update the peg count.

				//Test, if it is still a simple single wire, else don't add the wire and assume MWP:
				if(
					enteredStallMode //If in stall mode, then it must be MWP.
					|| CWPTrigger.Modificator.Held() //If mod is held, stall mode and MWP are about to happen.
					|| firstGroup.isNotSWP() //Group is either 2D/Expanded/White-/Black-listed/Skipped
				)
				{
					yield break;
				}
				PegAddress first = firstGroup.getFirstAxis().firstPeg;
				PegAddress second = firstGroup.getFirstAxis().secondPeg;
				if(second != null)
				{
					yield return (first, second, WireUtility.WireWouldBeValid(first, second));
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
			if(!secondGroup.isSet() && CWPSettings.connectPegsInOneGroupWithEachOther && (firstGroup.isTwoDimensional() || firstGroup.hasExtraPegs()))
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

		public static CWPGroup getFirstGroup()
		{
			return firstGroup;
		}

		public static CWPGroup getSecondGroup()
		{
			return secondGroup;
		}

		public static bool isCurrentlyEditingAxis()
		{
			return applyOnUp;
		}

		public static bool isCurrentlyInToggleListMode()
		{
			return toggleListMode;
		}
	}
}
