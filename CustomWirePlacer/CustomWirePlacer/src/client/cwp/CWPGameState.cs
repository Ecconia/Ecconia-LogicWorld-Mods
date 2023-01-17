using System;
using System.Collections.Generic;
using FancyInput;
using LogicWorld.GameStates;
using LogicWorld.Input;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGameState : GameState
	{
		public const string id = "CustomWirePlacer.WirePlacing";

		//Setup:

		public override bool MouseLocked => true;

		public override string TextID => id;

		public override IEnumerable<InputTrigger> HelpScreenTriggers => new InputTrigger[]
		{
			//Commented out, since LW does not show it by default and its intuitive.
			// (InputTrigger) (Enum) UITrigger.Back,
			(InputTrigger) (Enum) Trigger.CancelPlacing, //Not shown by default, but not intuitive, hence added.
			(InputTrigger) (Enum) CWPTrigger.OpenSettings,
			(InputTrigger) (Enum) CWPTrigger.GoTwoDimensional,
			(InputTrigger) (Enum) CWPTrigger.RemoveWires,
			(InputTrigger) (Enum) CWPTrigger.Flip,
			(InputTrigger) (Enum) CWPTrigger.ApplyPattern,
			(InputTrigger) (Enum) CWPTrigger.ApplyNormalAction,

			(InputTrigger) (Enum) CWPTrigger.SkipModeSwitch,
			(InputTrigger) (Enum) CWPTrigger.ExpandFurther,
			(InputTrigger) (Enum) CWPTrigger.ExpandBackwards,

			(InputTrigger) (Enum) CWPTrigger.Modificator,
			(InputTrigger) (Enum) CWPTrigger.ModificatorAlternative,
			(InputTrigger) (Enum) Trigger.DrawWire, //Obvious... hence at the bottom to be cropped.
		};

		//Routines:

		public override void OnEnter()
		{
			CustomWirePlacer.onActivate();
		}

		public override void OnRun()
		{
			CustomWirePlacer.onUpdate();
		}

		public override void OnExit()
		{
			CustomWirePlacer.onDeactivate();
		}
	}
}
