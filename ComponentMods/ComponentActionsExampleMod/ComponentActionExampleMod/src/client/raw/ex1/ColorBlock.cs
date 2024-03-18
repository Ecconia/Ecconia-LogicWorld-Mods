using LogicAPI.Data;
using LogicWorld.Rendering.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex1
{
	public class ColorBlock : ComponentClientCode
	{
		private static readonly GpuColor[] colors = new GpuColor[]
		{
			new GpuColor(0, 0, 0),
			new GpuColor(1, 0, 0),
			new GpuColor(0, 1, 0),
			new GpuColor(0, 0, 1),
			new GpuColor(1, 0, 1),
			new GpuColor(1, 1, 0),
			new GpuColor(0, 1, 1),
			new GpuColor(1, 1, 1),
		};
		
		protected override void Initialize()
		{
			// Initialize custom data, if it is not:
			if(Component.Data.CustomData == null)
			{
				((IEditableComponentData) Component.Data).CustomData = new byte[] {0};
			}
		}
		
		protected override void DataUpdate()
		{
			setColor(Component.Data.CustomData[0]);
		}
		
		public void onActionReceived(byte value)
		{
			//Updating the custom data is optional, becomes relevant if a client intends to save the world, so do the favor and save it k?
			Component.Data.CustomData[0] = value;
			setColor(value);
		}
		
		private void setColor(byte value)
		{
			if(value > colors.Length)
			{
				return;
			}
			SetBlockColor(colors[value], 0);
		}
	}
}
