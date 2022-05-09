using System.Collections.Generic;
using System.Linq;
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
			//We are not starting to draw a wire:
			startWireDrawing(pegCurrentlyLookingAt);
			return false;
		}

		private static void startWireDrawing(PegAddress initialPeg)
		{
			bool isBus = CWPTrigger.Modificator.Held(); //TBI: Use the official mod key instead?
			if(isBus && secondGroup.isSet())
			{
				//The former second group is now the first group, so that one can continue from there, however the references need to be swapped, since there are only two.
				(firstGroup, secondGroup) = (secondGroup, firstGroup);
				currentGroup = secondGroup; //Continue at second group.
				firstGroup.show(); //Set the outlines.
			}
			else if(isBus && firstGroup.isSet())
			{
				//We are in BUS mode, the first group is already finished, we start with the second group.
				currentGroup = secondGroup;
				firstGroup.show(); //Set the outlines.
			}
			else
			{
				//Cleanup leftover data:
				firstGroup.clear();
				currentGroup = firstGroup;
			}
			secondGroup.clear(); //In any case, the second group needs to be reset now.

			//Set the first peg:
			currentGroup.setFirstPeg(initialPeg);
			SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, CWPHelper.getWireConnectionPoint(initialPeg));

			//Switch state:
			GameStateManager.TransitionTo(CWPGameState.id);
		}

		public static void onActivate()
		{
			drawing = true; //Yes we are drawing!

			//Handle settings:
			if(CWPSettings.resetFlipping)
			{
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

			//Handle outlining:
			cleanUpWireGhosts();
			currentGroup = null;
			//Don't clear first and second group, since they might be used for Bus mode.
			//Hide them though:
			firstGroup.hide();
			secondGroup.hide();
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

			//Handle peg-selection and mouse-up while drawing:
			if(drawing)
			{
				PegAddress currentlyLookingAtPeg = CWPHelper.getPegCurrentlyLookingAt();
				if(Trigger.DrawWire.Held() && currentlyLookingAtPeg != null)
				{
					if(currentlyLookingAtPeg == currentGroup.getFirstPeg())
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

				if(Trigger.DrawWire.UpThisFrame())
				{
					drawing = false; //No longer drawing.
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
						return;
					}
					//Stall mode, here the group can be edited, or a new one started.
				}
			}
			else //Handle the start of the second group and applying of build action.
			{
				if(!secondGroup.isSet() && Trigger.DrawWire.DownThisFrame())
				{
					PegAddress lookingAt = CWPHelper.getPegCurrentlyLookingAt();
					if(lookingAt != null)
					{
						//Starting to draw the second group!
						drawing = true;
						secondGroup.setFirstPeg(lookingAt);
						currentGroup = secondGroup;
						updated = true;
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
					return;
				}
			}
			
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

			if(updated)
			{
				updateWireGhosts();
			}
		}

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
			else
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
			else
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
			GameStateManager.TransitionBackToBuildingState(); //Does the cleanup.
		}
	}
}
