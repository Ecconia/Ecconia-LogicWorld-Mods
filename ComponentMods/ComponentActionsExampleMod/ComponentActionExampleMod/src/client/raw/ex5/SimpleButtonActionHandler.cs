using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicWorld.Interfaces;
using LogicWorld.SharedCode.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex5
{
	public class SimpleButtonActionHandler : IComponentActionMutationHandler
	{
		public static void init()
		{
			ComponentActionMutationManager.RegisterHandler(new SimpleButtonActionHandler(), "ComponentActionExampleMod.SimpleButton");
		}
		
		public void HandleComponentAction(ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			var customData = (SimpleButton) Instances.MainWorld.Renderer.Entities.GetClientCode(componentAddress);
			customData.pressReceived(actionData[0]);
		}
	}
}
