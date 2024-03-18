using LogicAPI.Server.Components;

namespace ComponentActionExampleMod.Server.Raw.Ex2
{
	public class AddSubber : LogicComponent
	{
		protected override void DoLogicUpdate()
		{
			// Read input values:
			var bottom = readValue(0);
			var top = readValue(4);
			
			bool isAdd = ComponentData.CustomData[0] == 0;
			int result = isAdd ? bottom + top : bottom - top;
			
			Outputs[0].On = (result & 1) != 0;
			Outputs[1].On = (result & 2) != 0;
			Outputs[2].On = (result & 4) != 0;
			Outputs[3].On = (result & 8) != 0;
		}
		
		public void changeMode(byte mode)
		{
			var data = ComponentData.CustomData;
			if(data[0] == mode)
			{
				return; //Nothing to update.
			}
			//Update local custom data, such that it will be saved.
			ComponentData.CustomData[0] = mode;
			//Queue a logic update, so that the calculation updates according to the new mode.
			QueueLogicUpdate();
		}
		
		private int readValue(int offset, int amount = 4)
		{
			int value = 0;
			for(int i = offset + amount - 1; i >= offset; i--)
			{
				value <<= 1;
				value |= Inputs[i].On ? 1 : 0;
			}
			return value;
		}
	}
}
