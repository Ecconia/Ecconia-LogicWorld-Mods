using LogicAPI.Server.Components;

namespace ComponentActionExampleMod.Server.Raw.Ex3
{
	public class PulseByEdit : LogicComponent
	{
		protected override void DoLogicUpdate()
		{
			byte value = ComponentData.CustomData[0];
			if(value != 0)
			{
				ComponentData.CustomData[0] = (byte) (value - 1);
				QueueLogicUpdate(); //Run again next tick
			}
			Outputs[0].On = value != 0;
		}
		
		public void pulseForDuration(byte pulseLength)
		{
			//Update local custom data, such that the pulse is persistent over world loads.
			ComponentData.CustomData[0] = pulseLength;
			//Queue a logic update, so that a pulse will be emitted.
			QueueLogicUpdate();
		}
	}
}
