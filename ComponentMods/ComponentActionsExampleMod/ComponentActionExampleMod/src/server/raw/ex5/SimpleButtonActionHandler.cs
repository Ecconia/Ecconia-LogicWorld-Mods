using System.Collections.Generic;
using EccsLogicWorldAPI.Server;
using LogicAPI.Data;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server.Managers;

namespace ComponentActionExampleMod.Server.Raw.Ex5
{
	public class SimpleButtonActionHandler : ComponentActionBuildRequestHandler
	{
		public static void init()
		{
			var manager = ServiceGetter.getService<ComponentActionBuildRequestManager>();
			manager.RegisterHandler(generator, "ComponentActionExampleMod.SimpleButton");
		}
		
		private static ComponentActionBuildRequestHandler generator(bool prepareUndoRequests, ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			// Create a new handler for each request:
			return new SimpleButtonActionHandler(componentAddress, actionData);
		}
		
		private readonly ComponentAddress componentAddress;
		private readonly byte[] actionData;
		
		public SimpleButtonActionHandler(ComponentAddress componentAddress, byte[] actionData)
		{
			this.componentAddress = componentAddress;
			this.actionData = actionData;
		}
		
		public override bool CanDo()
		{
			return actionData.Length == 1 && actionData[0] < 2;
		}
		
		public override IEnumerable<WorldDataMutation> EnumerateMutationUpdates()
		{
			yield return new WorldMutation_SendComponentAction()
			{
				AddressOfTargetComponent = componentAddress,
				ActionData = actionData,
			};
		}
	}
}
