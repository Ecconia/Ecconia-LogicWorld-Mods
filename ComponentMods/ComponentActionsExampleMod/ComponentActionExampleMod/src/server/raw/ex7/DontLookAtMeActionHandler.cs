using System.Collections.Generic;
using EccsLogicWorldAPI.Server;
using LogicAPI.Data;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server.Managers;

namespace ComponentActionExampleMod.Server.Raw.Ex7
{
	public class DontLookAtMeActionHandler : ComponentActionBuildRequestHandler
	{
		public static void init()
		{
			var manager = ServiceGetter.getService<ComponentActionBuildRequestManager>();
			manager.RegisterHandler(generator, "ComponentActionExampleMod.DontLookAtMe");
		}
		
		private static ComponentActionBuildRequestHandler generator(bool prepareUndoRequests, ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			// Create a new handler for each request:
			return new DontLookAtMeActionHandler(componentAddress, actionData);
		}
		
		// The server is only forwarding this event, so do not attempt apply it locally.
		public override bool ApplyMutationUpdatesLocallyAfterBroadcasting { get; protected set; } = false;
		
		private readonly ComponentAddress componentAddress;
		private readonly byte[] actionData;
		
		public DontLookAtMeActionHandler(ComponentAddress componentAddress, byte[] actionData)
		{
			this.componentAddress = componentAddress;
			this.actionData = actionData;
		}
		
		public override bool CanDo()
		{
			return actionData == null;
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
