using System;
using System.Collections.Generic;
using FancyInput;
using LogicUI;
using LogicWorld.GameStates;
using LogicWorld.Input;

namespace CustomWirePlacer.Client.CWP.PegDrawing
{
	public class GameStatePegDrawing : GameState
	{
		public const string id = "CustomWirePlacer.PegDrawing";
		
		//Setup:
		
		public override bool PlayerCanMoveAndLookAround => true;
		
		public override string TextID => id;
		
		public override IEnumerable<InputTrigger> HelpScreenTriggers => new InputTrigger[]
		{
			(InputTrigger) (Enum) UITrigger.Back,
			(InputTrigger) (Enum) Trigger.CancelPlacing,
			(InputTrigger) (Enum) Trigger.DrawWires,
		};
		
		//Routines:
		
		public override void OnEnter()
		{
			PegDrawing.onActivate();
		}
		
		public override void OnRun()
		{
			PegDrawing.onUpdate();
		}
		
		public override void OnExit()
		{
			PegDrawing.onDeactivate();
			// Meh stuff, gotta be done to prevent weird issues with Tap-To-Toggle
			CustomInput.ResetState(Trigger.DrawWires);
		}
	}
}
