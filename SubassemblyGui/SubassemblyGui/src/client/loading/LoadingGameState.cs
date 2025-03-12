using LogicWorld.GameStates;

namespace SubassemblyGui.Client.loading
{
	public class LoadingGameState : GameState
	{
		public const string ID = "SubassemblyGui.Loading";
		
		public override string TextID => ID;
		public override bool MouseLocked => false;
		
		public override void OnEnter()
		{
			LoadingGui.ShowMenu();
		}
		
		public override void OnRun()
		{
			LoadingGui.Instance.OnRun();
		}
		
		public override void OnExit()
		{
			LoadingGui.HideMenu();
		}
	}
}
