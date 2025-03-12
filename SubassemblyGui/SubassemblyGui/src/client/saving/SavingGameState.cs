using LogicUI;
using LogicWorld.GameStates;

namespace SubassemblyGui.Client.Saving
{
	public class SavingGameState : GameState
	{
		public const string ID = "SubassemblyGui.Saving";
		
		public override bool MouseLocked => false;
		public override string TextID => ID;
		
		public override void OnEnter()
		{
			SavingGui.ShowMenu();
		}
		
		public override void OnRun()
		{
			if (UITrigger.Back.DownThisFrame())
			{
				GameStateManager.TransitionBackToBuildingState();
			}
		}
		
		public override void OnExit()
		{
			SavingGui.HideMenu();
		}
	}
}
