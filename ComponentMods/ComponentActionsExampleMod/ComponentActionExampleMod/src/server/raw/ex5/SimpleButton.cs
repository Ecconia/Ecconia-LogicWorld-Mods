using LogicAPI.Server.Components;

namespace ComponentActionExampleMod.Server.Raw.Ex5
{
	public class SimpleButton : LogicComponent
	{
		private bool targetState;
		
		protected override void DoLogicUpdate()
		{
			Outputs[0].On = targetState;
		}
		
		public void pressed(bool isPressed)
		{
			targetState = isPressed;
			QueueLogicUpdate();
		}
	}
}
