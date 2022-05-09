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
		private static CWPGroup firstCwpGroup = new CWPGroup();
		private static CWPGroup secondCwpGroup = new CWPGroup();
		//The current group is used to reference the group which currently is being modified.
		private static CWPGroup currentCwpGroup;
		
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
			//TODO: Handle MOD key usage for the BUS feature.

			//Cleanup leftover data:
			firstCwpGroup.clear();
			secondCwpGroup.clear();

			//Set the first peg:
			firstCwpGroup.setFirstPeg(initialPeg);
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
			CWPSettings.skiprate = 0; //This gets reset before each operation.

			currentCwpGroup = firstCwpGroup;

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
			//Hard reset fields:
			firstCwpGroup.clear();
			secondCwpGroup.clear();
			currentCwpGroup = null;
		}

		public static void onUpdate()
		{
			if(UITrigger.Back.DownThisFrame() || Trigger.CancelPlacing.DownThisFrame())
			{
				//The user does not want to finish any wire placement action.
				// Undo whatever has been done already:
				//TBI: Handle this instead on game state deactivate?
				// Done here, lets leave:
				SoundPlayer.PlayFail();
				GameStateManager.TransitionBackToBuildingState();
				return;
			}

			if(drawing)
			{
				PegAddress currentlyLookingAtPeg = CWPHelper.getPegCurrentlyLookingAt();
				if(Trigger.DrawWire.Held() && currentlyLookingAtPeg != null)
				{
					if(currentlyLookingAtPeg == currentCwpGroup.getFirstPeg())
					{
						if(currentCwpGroup.getSecondPeg() != null)
						{
							//Peg reset:
							currentCwpGroup.setSecondPeg(null);
							if(secondCwpGroup.isSet())
							{
								//In the first group, we are not making the sound, since then no wire is drawn at all.
								// But when a second group exists, the wire will update and a sound has to be played.
								SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, currentlyLookingAtPeg);
							}
							updateWireGhosts();
						}
					}
					else if(currentlyLookingAtPeg != currentCwpGroup.getSecondPeg())
					{
						//Peg switched:
						currentCwpGroup.setSecondPeg(currentlyLookingAtPeg);
						SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, currentlyLookingAtPeg);
						updateWireGhosts();
					}
				}

				if(Trigger.DrawWire.UpThisFrame())
				{
					drawing = false; //No longer drawing.
					if(!Trigger.Mod.Held())
					{
						applyNormalAction();
						return;
					}
					//Stall mode, here the group can be edited, or a new one started.
				}
			}
			else
			{
				if(!secondCwpGroup.isSet() && Trigger.DrawWire.DownThisFrame())
				{
					PegAddress lookingAt = CWPHelper.getPegCurrentlyLookingAt();
					if(lookingAt != null)
					{
						//Starting to draw the second group!
						drawing = true;
						secondCwpGroup.setFirstPeg(lookingAt);
						currentCwpGroup = secondCwpGroup;
						updateWireGhosts();
					}
				}
				//Else this click is for now meaningless.

				if(CWPTrigger.ApplyNormalAction.UpThisFrame())
				{
					applyNormalAction();
					return;
				}
			}

			if(CWPTrigger.OpenSettings.DownThisFrame())
			{
				CWPSettingsWindow.toggleVisibility();
			}

			if(Trigger.Flip.DownThisFrame())
			{
				CWPSettings.flipping = !CWPSettings.flipping;
				updateWireGhosts();
			}
		}

		private static void updateWireGhosts()
		{
			cleanUpWireGhosts();
			if(secondCwpGroup.isSet())
			{
				List<PegAddress> smaller = firstCwpGroup.getPegs().ToList();
				List<PegAddress> bigger = secondCwpGroup.getPegs().ToList();
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
				if(firstCwpGroup.hasExtraPegs())
				{
					valid = CWPOutlineData.validMultiWire;
					invalid = CWPOutlineData.invalidMultiWire;
				}
				//Drawing 1-group connections:
				IEnumerator<PegAddress> it = firstCwpGroup.getPegs().GetEnumerator();
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
			if(secondCwpGroup.isSet())
			{
				//Two groups!
				List<BuildRequest> requests = new List<BuildRequest>();
				{
					List<PegAddress> smaller = firstCwpGroup.getPegs().ToList();
					List<PegAddress> bigger = secondCwpGroup.getPegs().ToList();
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
						requests.Add(new BuildRequest_CreateWire(new WireData(smaller[i], bigger[i], 0f)));
					}
					PegAddress constant = smaller[smaller.Count - 1];
					for(int i = smaller.Count; i < bigger.Count; i++)
					{
						requests.Add(new BuildRequest_CreateWire(new WireData(constant, bigger[i], 0f)));
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
				if(firstCwpGroup.getSecondPeg() != null)
				{
					//We have more than 1 peg.
					if(firstCwpGroup.hasExtraPegs())
					{
						//We are placing more than 1 wire:
						List<BuildRequest> requests = new List<BuildRequest>();
						IEnumerator<PegAddress> it = firstCwpGroup.getPegs().GetEnumerator();
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
						if(WireUtility.WireWouldBeValid(firstCwpGroup.getFirstPeg(), firstCwpGroup.getSecondPeg()))
						{
							BuildRequestManager.SendBuildRequest(new BuildRequest_CreateWire(new WireData(firstCwpGroup.getFirstPeg(), firstCwpGroup.getSecondPeg(), 0f)));
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
