using LogicAPI.Data;
using LogicWorld.Rendering.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex2
{
	public class AddSubber : ComponentClientCode
	{
		protected override void Initialize()
		{
			// Initialize custom data, if it is not:
			if(Component.Data.CustomData == null)
			{
				((IEditableComponentData) Component.Data).CustomData = new byte[] {0};
			}
		}
		
		// The main disadvantage of this example is, that the client has no clue about custom data changes on the client side.
		// This means, that when one intends to save the world or a subassembly locally, the data might be outdated and wrong.
		// Players can rejoin, to get a relatively up to date state though - preferably with simulation paused.
	}
}
