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
		
		public override bool MouseLocked => true;
		
		public override string TextID => id;
		
		public override IEnumerable<InputTrigger> HelpScreenTriggers => new InputTrigger[]
		{
			UITrigger.Back,
			Trigger.CancelPlacing,
			Trigger.DrawWire,
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
		}
	}
}
