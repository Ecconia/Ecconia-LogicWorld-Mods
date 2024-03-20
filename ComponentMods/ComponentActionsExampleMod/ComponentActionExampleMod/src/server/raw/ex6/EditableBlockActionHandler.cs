using System.Collections.Generic;
using ComponentActionExampleMod.shared.ex6;
using EccsLogicWorldAPI.Server;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server.Managers;

namespace ComponentActionExampleMod.Server.Raw.Ex6
{
	public static class EditableBlockActionHandler
	{
		public static void init()
		{
			var manager = ServiceGetter.getService<ComponentActionBuildRequestManager>();
			manager.RegisterHandler(generator, "ComponentActionExampleMod.EditableBlock");
		}
		
		private static ComponentActionBuildRequestHandler generator(bool prepareUndoRequests, ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			// Parse the action up front
			var parsedAction = EditableComponentCustomData.parseAction(actionData);
			if(!parsedAction.HasValue)
			{
				return new EditableBlockActionHandler_CantDo();
			}
			var (height, color) = parsedAction.Value;
			if(height.HasValue)
			{
				return new EditableBlockActionHandler_Height(componentAddress, componentInWorld, prepareUndoRequests, actionData, height.Value);
			}
			else
			{
				return new EditableBlockActionHandler_Color(componentAddress, componentInWorld, prepareUndoRequests, actionData, color.Value);
			}
		}
		
		//Parsing of data failed, just return a rejecting handler.
		private class EditableBlockActionHandler_CantDo : ComponentActionBuildRequestHandler
		{
			public override bool CanDo()
			{
				return false;
			}
		}
		
		private class EditableBlockActionHandler_Height : ComponentActionBuildRequestHandler
		{
			// This will not make the API attempt to apply the mutation locally - has to be applied manually.
			public override bool ApplyMutationUpdatesLocallyAfterBroadcasting { get; protected set; } = false;
			
			private readonly ComponentAddress address;
			private readonly IComponentInWorld component;
			private readonly bool prepareUndo;
			private readonly byte[] validUpdateAction;
			private readonly float newHeight;
			
			public EditableBlockActionHandler_Height(ComponentAddress address, IComponentInWorld component, bool prepareUndo, byte[] validUpdateAction, float newHeight)
			{
				this.address = address;
				this.component = component;
				this.newHeight = newHeight;
				this.prepareUndo = prepareUndo;
				this.validUpdateAction = validUpdateAction;
			}
			
			public override bool CanDo()
			{
				// Data validation, we don't want to go too small.
				return newHeight >= 1;
			}
			
			public override IEnumerable<WorldDataMutation> EnumerateMutationUpdates()
			{
				yield return new WorldMutation_SendComponentAction()
				{
					AddressOfTargetComponent = address,
					ActionData = validUpdateAction,
				};
			}
			
			private float previousHeight;
			
			public override void PostMutationBroadcast()
			{
				if(prepareUndo)
				{
					previousHeight = EditableComponentCustomData.extractHeight(component.Data.CustomData);
				}
				// Apply to local custom data, so that it gets saved:
				EditableComponentCustomData.inject(newHeight, component.Data.CustomData);
			}
			
			public override IEnumerable<BuildRequest> EnumerateUndoRequests()
			{
				yield return new BuildRequest_SendComponentAction(address, EditableComponentCustomData.getActionFor(previousHeight));
			}
		}
		
		private class EditableBlockActionHandler_Color : ComponentActionBuildRequestHandler
		{
			// This will not make the API attempt to apply the mutation locally - has to be applied manually.
			public override bool ApplyMutationUpdatesLocallyAfterBroadcasting { get; protected set; } = false;
			
			private readonly ComponentAddress address;
			private readonly IComponentInWorld component;
			private readonly bool prepareUndo;
			private readonly byte[] validUpdateAction;
			private readonly Color24 newColor;
			
			public EditableBlockActionHandler_Color(ComponentAddress address, IComponentInWorld component, bool prepareUndo, byte[] validUpdateAction, Color24 newColor)
			{
				this.address = address;
				this.component = component;
				this.newColor = newColor;
				this.prepareUndo = prepareUndo;
				this.validUpdateAction = validUpdateAction;
			}
			
			public override IEnumerable<WorldDataMutation> EnumerateMutationUpdates()
			{
				yield return new WorldMutation_SendComponentAction()
				{
					AddressOfTargetComponent = address,
					ActionData = validUpdateAction,
				};
			}
			
			private Color24 previousColor;
			
			public override void PostMutationBroadcast()
			{
				if(prepareUndo)
				{
					previousColor = EditableComponentCustomData.extractColor(component.Data.CustomData);
				}
				// Apply to local custom data, so that it gets saved:
				EditableComponentCustomData.inject(newColor, component.Data.CustomData);
			}
			
			public override IEnumerable<BuildRequest> EnumerateUndoRequests()
			{
				yield return new BuildRequest_SendComponentAction(address, EditableComponentCustomData.getActionFor(previousColor));
			}
		}
	}
}
