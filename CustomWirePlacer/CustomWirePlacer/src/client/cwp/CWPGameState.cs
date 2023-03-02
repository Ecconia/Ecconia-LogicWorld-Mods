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
			// UITrigger.Back,
			Trigger.CancelPlacing, //Not shown by default, but not intuitive, hence added.
			CWPTrigger.OpenSettings,
			CWPTrigger.GoTwoDimensional,
			CWPTrigger.RemoveWires,
			CWPTrigger.Flip,
			CWPTrigger.ApplyPattern,
			CWPTrigger.ApplyNormalAction,

			CWPTrigger.SkipModeSwitch,
			CWPTrigger.ExpandFurther,
			CWPTrigger.ExpandBackwards,

			CWPTrigger.Modificator,
			CWPTrigger.ModificatorAlternative,
			Trigger.DrawWire, //Obvious... hence at the bottom to be cropped.
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
