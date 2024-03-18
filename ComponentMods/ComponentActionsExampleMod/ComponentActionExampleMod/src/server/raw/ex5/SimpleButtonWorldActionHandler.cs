using EccsLogicWorldAPI.Server;
using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicWorld.Server.Circuitry;
using LogicWorld.SharedCode.Components;

namespace ComponentActionExampleMod.Server.Raw.Ex5
{
	public class SimpleButtonWorldActionHandler : IComponentActionMutationHandler
	{
		private static ICircuitryManager circuitryManager;
		
		public static void init()
		{
			circuitryManager = ServiceGetter.getService<ICircuitryManager>();
			ComponentActionMutationManager.RegisterHandler(new SimpleButtonWorldActionHandler(), "ComponentActionExampleMod.SimpleButton");
		}
		
		public void HandleComponentAction(ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			var button = (SimpleButton) circuitryManager.LookupComponent(componentAddress);
			button.pressed(actionData[0] != 0);
		}
	}
}
