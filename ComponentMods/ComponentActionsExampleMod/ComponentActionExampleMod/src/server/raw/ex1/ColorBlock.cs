using EccsLogicWorldAPI.Server;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server.Managers;

namespace ComponentActionExampleMod.Server.Raw.Ex1
{
	public class ColorBlock : LogicComponent
	{
		private static readonly IWorldUpdates updateSender;
		
		static ColorBlock()
		{
			updateSender = ServiceGetter.getService<IWorldUpdates>();
		}
		
		private bool wasOn;
		
		protected override void DoLogicUpdate()
		{
			if(!Inputs[3].On)
			{
				wasOn = false;
				return;
			}
			else if(wasOn)
			{
				return;
			}
			wasOn = true;
			// Detected a rising edge, do the actual work now.
			
			byte value = (byte) (
				(Inputs[0].On ? 0b1 : 0)
				+ (Inputs[1].On ? 0b10 : 0)
				+ (Inputs[2].On ? 0b100 : 0)
			);
			
			if(ComponentData.CustomData[0] == value)
			{
				return; // Value is up to date.
			}
			// Update value:
			ComponentData.CustomData[0] = value;
			
			// In this example the custom data is only a single byte, you might as well use the original
			//  custom data framework. This becomes interesting, if you send something that is not backed in custom data.
			// Or you have much more in custom data, which does not need to be sent.
			
			updateSender.QueueMutationToBeSentToClient(new WorldMutation_SendComponentAction()
			{
				AddressOfTargetComponent = Address,
				ActionData = ComponentData.CustomData,
			});
		}
	}
}
