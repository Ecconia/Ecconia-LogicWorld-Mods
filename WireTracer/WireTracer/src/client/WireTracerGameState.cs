using System.Collections.Generic;
using FancyInput;
using LogicWorld.GameStates;
using LogicWorld.Input;
using WireTracer.Client.Keybindings;
using WireTracer.Client.Tool;

namespace WireTracer.Client
{
	public class WireTracerGameState : GameState
	{
		public const string id = "WireTracer.WireTracing";
		
		//Setup:
		
		public override bool PlayerCanMoveAndLookAround => true;
		
		public override string TextID => id;
		
		public override IEnumerable<InputTrigger> HelpScreenTriggers => new InputTrigger[]
		{
			Trigger.CancelPlacing,
			WireTracerTrigger.HighlightCluster,
		};
		
		//Routines:
		
		public override void OnEnter()
		{
			WireTracerTool.onStart();
		}
		
		public override void OnRun()
		{
			WireTracerTool.onUpdate();
		}
		
		public override void OnExit()
		{
			WireTracerTool.onStop();
		}
	}
}
