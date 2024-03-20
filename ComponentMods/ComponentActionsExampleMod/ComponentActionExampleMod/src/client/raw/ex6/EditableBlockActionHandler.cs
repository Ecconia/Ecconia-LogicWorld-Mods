using System;
using ComponentActionExampleMod.shared.ex6;
using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicWorld.Interfaces;
using LogicWorld.SharedCode.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex6
{
	public class EditableBlockActionHandler : IComponentActionMutationHandler
	{
		public static event Action<ComponentAddress> onBlockHeightEdited;
		
		public static void init()
		{
			ComponentActionMutationManager.RegisterHandler(new EditableBlockActionHandler(), "ComponentActionExampleMod.EditableBlock");
		}
		
		public void HandleComponentAction(ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			var parsedAction = EditableComponentCustomData.parseAction(actionData);
			if(!parsedAction.HasValue)
			{
				return; //Whoops broken data.
			}
			var customData = (EditableBlock) Instances.MainWorld.Renderer.Entities.GetClientCode(componentAddress);
			var (height, color) = parsedAction.Value;
			if(height.HasValue)
			{
				customData.setHeight(height.Value);
				// The outline of the editing window might not be updated... have this callback for the edit window to react.
				onBlockHeightEdited?.Invoke(componentAddress);
			}
			else
			{
				customData.setColor(color.Value);
			}
		}
	}
}
