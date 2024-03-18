using EccsLogicWorldAPI.Server;
using LogicAPI.Data;
using LogicAPI.Server.Components;
using LogicWorld.Server.Circuitry;
using LogicWorld.Server.Managers;

namespace ComponentActionExampleMod.Server.Raw.Ex3
{
	public class PulseByEditActionHandler : ComponentActionBuildRequestHandler
	{
		private static ICircuitryManager circuitryManager;
		
		public static void init()
		{
			circuitryManager = ServiceGetter.getService<ICircuitryManager>();
			var manager = ServiceGetter.getService<ComponentActionBuildRequestManager>();
			manager.RegisterHandler(generator, "ComponentActionExampleMod.PulseByEdit");
		}
		
		private static ComponentActionBuildRequestHandler generator(bool prepareUndoRequests, ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			// Create a new handler for each request:
			return new PulseByEditActionHandler(componentAddress, actionData);
		}
		
		private readonly ComponentAddress componentAddress;
		private readonly byte[] actionData;
		
		public PulseByEditActionHandler(ComponentAddress componentAddress, byte[] actionData)
		{
			this.componentAddress = componentAddress;
			this.actionData = actionData;
		}
		
		public override bool CanDo()
		{
			return actionData.Length == 1;
		}
		
		// There are no mutations in this class, it entirely only forwards the change to the server component.
		// This is not ideal for a real case scenario, but this is just an example.
		
		public override void PostMutationBroadcast()
		{
			// By design, logic component should exist for the type of this component. A cast is thus not an issue.
			var component = (PulseByEdit) circuitryManager.LookupComponent(componentAddress);
			component.pulseForDuration(actionData[0]);
		}
	}
}
