using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicWorld.Interfaces;
using LogicWorld.SharedCode.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex1
{
	public class ColorBlockActionHandler : IComponentActionMutationHandler
	{
		public static void init()
		{
			ComponentActionMutationManager.RegisterHandler(new ColorBlockActionHandler(), "ComponentActionExampleMod.ColorBlock");
		}
		
		public void HandleComponentAction(ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			//Assume that, the type is correct. Also assuming that the component has component data.
			// Cause the ComponentAction API and the mod design ensure this.
			var customData = (ColorBlock) Instances.MainWorld.Renderer.Entities.GetClientCode(componentAddress);
			customData.onActionReceived(actionData[0]);
		}
	}
}
