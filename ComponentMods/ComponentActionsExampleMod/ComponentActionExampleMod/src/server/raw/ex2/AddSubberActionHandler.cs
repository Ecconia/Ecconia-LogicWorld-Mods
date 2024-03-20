using System.Collections.Generic;
using EccsLogicWorldAPI.Server;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicAPI.Server.Components;
using LogicWorld.Server.Circuitry;
using LogicWorld.Server.Managers;

namespace ComponentActionExampleMod.Server.Raw.Ex2
{
	public class AddSubberActionHandler : ComponentActionBuildRequestHandler
	{
		private static ICircuitryManager circuitryManager;
		
		public static void init()
		{
			circuitryManager = ServiceGetter.getService<ICircuitryManager>();
			var manager = ServiceGetter.getService<ComponentActionBuildRequestManager>();
			manager.RegisterHandler(generator, "ComponentActionExampleMod.AddSubber");
		}
		
		private static ComponentActionBuildRequestHandler generator(bool prepareUndoRequests, ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			// Create a new handler for each request:
			return new AddSubberActionHandler(componentAddress, actionData);
		}
		
		private readonly ComponentAddress componentAddress;
		private readonly byte[] actionData;
		
		public AddSubberActionHandler(ComponentAddress componentAddress, byte[] actionData)
		{
			this.componentAddress = componentAddress;
			this.actionData = actionData;
		}
		
		public override bool CanDo()
		{
			return actionData.Length == 1 && actionData[0] < 2;
		}
		
		// There are no mutations in this class, it entirely only forwards the change to the server component.
		// This is not ideal for a real case scenario, but this is just an example.
		
		public override void PostMutationBroadcast()
		{
			// By design, logic component should exist for the type of this component. A cast is thus not an issue.
			var addSubber = (AddSubber) circuitryManager.LookupComponent(componentAddress);
			addSubber.changeMode(actionData[0]);
		}
		
		public override IEnumerable<BuildRequest> EnumerateUndoRequests()
		{
			yield return new BuildRequest_SendComponentAction(componentAddress, new byte[]
			{
				(byte) (actionData[0] == 0 ? 1 : 0),
			});
		}
	}
}
