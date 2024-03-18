using LogicAPI.Data;
using LogicWorld.Rendering.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex3
{
	public class PulseByEdit : ComponentClientCode
	{
		protected override void Initialize()
		{
			// Initialize custom data, if it is not:
			if(Component.Data.CustomData == null)
			{
				((IEditableComponentData) Component.Data).CustomData = new byte[] {0};
			}
		}
	}
}
