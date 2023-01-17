using System;
using System.Text;
using CustomWirePlacer.Client.CWP;
using CustomWirePlacer.Client.CWP.PegDrawing;
using EccsWindowHelper.Client;
using LogicLocalization;
using LogicUI.MenuTypes;
using LogicWorld.GameStates;
using LogicWorld.UI;
using LogicWorld.UI.HelpList;
using UnityEngine;

namespace CustomWirePlacer.Client.Windows
{
	public static class CWPHelpOverlay
	{
		private static GameObject rootObject;

		private static TextOverlayBox staticBox;
		private static TextOverlayBox dynamicBox;

		public static void Init()
		{
			//Static initialization, create the game object, which this window is attached to.
			if(rootObject != null)
			{
				throw new Exception("Already initialized CWP-HelpOverlay");
			}
			rootObject = WindowHelper.makeOverlayCanvas("CWP: HelpOverlay root");

			initConstantActionBox();
			initDynamicActionBox();

			//Listeners to manage visibility:
			//Help overlay listener:
			ToggleableSingletonMenu<HelpListMenu>.OnMenuShown += checkVisibility;
			ToggleableSingletonMenu<HelpListMenu>.OnMenuHidden += checkVisibility;
			//BuildingCanvas overlay listener:
			ToggleableSingletonMenu<BuildingCanvas>.OnMenuShown += checkVisibility;
			ToggleableSingletonMenu<BuildingCanvas>.OnMenuHidden += checkVisibility;
			//GameState listener:
			GameStateManager.OnHelpUpdated += gameState =>
			{
				State newState = State.Disabled;
				if(gameState != null)
				{
					string id = gameState.TextID;
					if("MHG.Building".Equals(id))
					{
						newState = State.Building;
					}
					else if(CWPGameState.id.Equals(id))
					{
						newState = State.CustomWirePlacer;
					}
					else if(GameStatePegDrawing.id.Equals(id))
					{
						newState = State.PegDrawing;
					}
				}
				if(newState != currentState)
				{
					currentState = newState;
					checkVisibility();
				}
			};

			TextLocalizer.Initialize(new LocalizationHelpUpdater());
		}

		private class LocalizationHelpUpdater : ILocalizedObject
		{
			public void UpdateLocalization()
			{
				checkVisibility();
			}
		}

		//Manage the visibility:

		private enum State
		{
			Disabled,
			Building,
			PegDrawing,
			CustomWirePlacer,
		}

		private static State currentState;

		public static void checkVisibility()
		{
			//Only show the overlay, if we have it enabled and the game-state is whitelisted:
			if(currentState != State.Disabled && CWPSettings.showHelpOverlay)
			{
				//Set the overlay up for CWP GS:
				if(currentState == State.CustomWirePlacer)
				{
					rootObject.SetActive(true);
					dynamicBox.setActive(true);
					if(CWP.CustomWirePlacer.isActive())
					{
						//By the time this is called by the listener, CWP is not yet active and hence not initialized.
						// Then the text may not be set. But if it is marked as active, it is initialized.
						setCWPText();
					}
					return;
				}
				//Set the overlay up for Building GS: (But only if F6 is enabled).
				if(currentState == State.Building && ToggleableSingletonMenu<HelpListMenu>.MenuIsVisible && ToggleableSingletonMenu<BuildingCanvas>.MenuIsVisible)
				{
					rootObject.SetActive(true);
					dynamicBox.setActive(false);
					setBuildingText();
					return;
				}
				//Set the overlay up for PegDrawing GS:
				if(currentState == State.PegDrawing)
				{
					rootObject.SetActive(true);
					dynamicBox.setActive(true);
					setPegDrawingText();
					return;
				}
			}
			rootObject.SetActive(false);
		}

		//Init the text boxes structure:

		private static void initConstantActionBox()
		{
			staticBox = new TextOverlayBox(rootObject, "CWP: Static Help Overlay");
			RectTransform rectTransform = staticBox.getRect();
			rectTransform.anchorMin = new Vector2(1, 1);
			rectTransform.anchorMax = new Vector2(1, 1);
			rectTransform.pivot = new Vector2(1, 1);
		}

		private static void initDynamicActionBox()
		{
			dynamicBox = new TextOverlayBox(rootObject, "CWP: Dynamic Help Overlay");
			RectTransform rectTransform = dynamicBox.getRect();
			rectTransform.anchorMin = new Vector2(0.5f, 1);
			rectTransform.anchorMax = new Vector2(0.5f, 1);
			rectTransform.pivot = new Vector2(0.5f, 1);
		}

		private static void setBuildingText()
		{
			staticBox.setText(TextLocalizer.LocalizedFormat("CWP.HelpOverlay.Building.StartWP", constructKeybinding("MHG.DrawWire")) + '\n' +
			                  TextLocalizer.LocalizedFormat("CWP.HelpOverlay.Building.StartPD", constructKeybinding("CustomWirePlacer.ModificatorAlternative")));
		}

		private static void setPegDrawingText()
		{
			dynamicBox.setText(TextLocalizer.LocalizedFormat("CWP.HelpOverlay.PegDrawing.Finish", constructKeybinding("MHG.DrawWire")));
			staticBox.setText(TextLocalizer.LocalizedFormat("CWP.HelpOverlay.CWP.Abort", constructKeybinding("LogicUI.Back"), constructKeybinding("MHG.CancelPlacing")));
		}

		public static void updateText()
		{
			if(!CWPSettings.showHelpOverlay)
			{
				return;
			}
			setCWPText();
		}

		private static void setCWPText()
		{
			//TODO: It is majorly inefficient to lookup and format the keys every refresh. Improve! (Eventually).

			StringBuilder sb = new StringBuilder();
			append(sb, "CWP.HelpOverlay.CWP.Abort", "LogicUI.Back", "MHG.CancelPlacing");
			append(sb, "CWP.HelpOverlay.CWP.Settings", "CustomWirePlacer.OpenSettings");
			append(sb, "CWP.HelpOverlay.CWP.Flip", "CustomWirePlacer.Flip");
			append(sb, "CWP.HelpOverlay.CWP.SkipModeSwitch", "CustomWirePlacer.SkipModeSwitch");
			if(CWP.CustomWirePlacer.getSecondGroup().isSet())
			{
				//If there are two groups:
				append(sb, "CWP.HelpOverlay.CWP.RemoveMulti", "CustomWirePlacer.RemoveWires");
			}
			else
			{
				//If there is only one group:
				append(sb, "CWP.HelpOverlay.CWP.RemoveSingle", "CustomWirePlacer.RemoveWires");
			}
			if(CWP.CustomWirePlacer.getCurrentGroup().getCurrentAxis().secondPeg != null)
			{
				//If second peg present:
				append(sb, "CWP.HelpOverlay.CWP.ForwardsStart", "CustomWirePlacer.ExpandFurther");
				append(sb, "CWP.HelpOverlay.CWP.BackwardsStart", "CustomWirePlacer.ExpandBackwards");
			}
			if(!CWP.CustomWirePlacer.getCurrentGroup().isTwoDimensional())
			{
				//If the current group is not two dimensional:
				append(sb, "CWP.HelpOverlay.CWP.GoTwoDimensional", "CustomWirePlacer.GoTwoDimensional");
			}
			if(CWP.CustomWirePlacer.getSecondGroup().isSet() || !CWP.CustomWirePlacer.isCurrentlyEditingAxis())
			{
				//If not editing the first group:
				if(CWP.CustomWirePlacer.isCurrentlyEditingAxis())
				{
					//If editing (second group):
					append(sb, "CWP.HelpOverlay.CWP.PatternNow", "CustomWirePlacer.ApplyPattern");
				}
				else
				{
					//If in stall mode:
					append(sb, "CWP.HelpOverlay.CWP.PatternNext", "CustomWirePlacer.ApplyPattern");
				}
			}
			staticBox.setText(sb.ToString());

			// OTHER :

			sb.Clear();
			if(CWP.CustomWirePlacer.isCurrentlyInToggleListMode())
			{
				append(sb, "CWP.HelpOverlay.CWP.BackToStall", "MHG.DrawWire");
			}
			else if(CWP.CustomWirePlacer.isCurrentlyEditingAxis())
			{
				append(sb, "CWP.HelpOverlay.CWP.FinishCWPNormally", "MHG.DrawWire");
				if(CWP.CustomWirePlacer.getSecondGroup().isSet() || CWPSettings.allowStartingWithOnePegGroup || CWP.CustomWirePlacer.getFirstGroup().hasMultiplePegs())
				{
					append(sb, "CWP.HelpOverlay.CWP.GoStallMode", "MHG.DrawWire", "CustomWirePlacer.Modificator");
				}
			}
			else
			{
				if(CWP.CustomWirePlacer.pendingTwoDimensional)
				{
					append(sb, "CWP.HelpOverlay.CWP.StartSecondAxis", "MHG.DrawWire");
				}
				else if(CWP.CustomWirePlacer.getSecondGroup().isSet())
				{
					append(sb, "CWP.HelpOverlay.CWP.ExBUSMode", "MHG.DrawWire");
				}
				else
				{
					append(sb, "CWP.HelpOverlay.CWP.ExNextGroup", "MHG.DrawWire");
				}
				append(sb, "CWP.HelpOverlay.CWP.ExToggleList", "MHG.DrawWire", "CustomWirePlacer.ModificatorAlternative");
				append(sb, "CWP.HelpOverlay.CWP.FinishCWPWithKey", "CustomWirePlacer.ApplyNormalAction");
			}
			bool allowSkipping = true;
			if(CWP.CustomWirePlacer.getCurrentGroup().getCurrentAxis().secondPeg != null)
			{
				if(CWPTrigger.ExpandFurther.Held())
				{
					allowSkipping = false;
					if(!CWP.CustomWirePlacer.doNotApplyExpandForward)
					{
						append(sb, "CWP.HelpOverlay.CWP.ForwardStop", "CustomWirePlacer.ExpandFurther");
					}
					append(sb, "CWP.HelpOverlay.CWP.NStepForward", "CustomWirePlacer.GenericMouseWheel");
				}
				if(CWPTrigger.ExpandBackwards.Held())
				{
					allowSkipping = false;
					if(!CWP.CustomWirePlacer.doNotApplyExpandBackwards)
					{
						append(sb, "CWP.HelpOverlay.CWP.BackwardStop", "CustomWirePlacer.ExpandBackwards");
					}
					append(sb, "CWP.HelpOverlay.CWP.NStepBackward", "CustomWirePlacer.GenericMouseWheel");
				}
			}
			if(allowSkipping)
			{
				append(sb, "CWP.HelpOverlay.CWP.SkipValue", "CustomWirePlacer.GenericMouseWheel");
				append(sb, "CWP.HelpOverlay.CWP.SkipOffset", "CustomWirePlacer.GenericMouseWheel", "CustomWirePlacer.ModificatorAlternative");
			}
			dynamicBox.setText(sb.ToString());
		}

		private static void append(StringBuilder sb, string key, string arg0)
		{
			sb.Append(TextLocalizer.LocalizedFormat(key, constructKeybinding(arg0)).Replace("\\n", "\n")).Append('\n');
		}

		private static void append(StringBuilder sb, string key, string arg0, string arg1)
		{
			sb.Append(TextLocalizer.LocalizedFormat(key, constructKeybinding(arg0), constructKeybinding(arg1)).Replace("\\n", "\n")).Append('\n');
		}

		private static string constructKeybinding(string key)
		{
			string keyName = TextLocalizer.LocalizeByKey("FancyInput.Trigger." + key);
			return "<mark=#ffffff44 padding=\"20, 20, 0, 0\"><b>[" + keyName + "]</b></mark>";
		}
	}
}
