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
			(InputTrigger) (Enum) Trigger.DrawWire,
			(InputTrigger) (Enum) Trigger.Mod,
			(InputTrigger) (Enum) CWPTrigger.OpenSettings,
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
