using System.Collections.Generic;
using System.Linq;
using ComponentActionExampleMod.Client.Raw.Ex6;
using ComponentActionExampleMod.shared.ex6;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicUI.ColorChoosing;
using LogicUI.MenuParts;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;
using LogicWorld.Outlines;
using LogicWorld.UI;

namespace ComponentActionExampleModGui.Client.Ex6
{
	public class CustomEditingComponentData : EditingComponentInfo
	{
		public float previousHeight;
		public Color24 previousColor;
	}
	
	public class EditEditableBlock : EditComponentMenuBase<CustomEditingComponentData>, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("ComponentActionExampleModGui.EditableBlock")
				.setYPosition(475)
				.configureContent(content => content
					.layoutVertical()
					.add(WS.colorPicker
						.injectionKey(nameof(colorPicker))
						.fixedSize(210, 70)
					)
					.add(WS.slider
						.injectionKey(nameof(heightSlider))
						.setMin(1f)
						.setInterval(0.5f)
						.setMax(3f)
						.setDecimalDigitsToDisplay(1)
						.fixedSize(500, 45)
					)
				)
				.add<EditEditableBlock>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		private ColorChooser colorPicker;
		
		[AssignMe]
		private InputSlider heightSlider;
		
		public override void Initialize()
		{
			base.Initialize();
			
			colorPicker.OnColorChange24 += color => {
				broadcastBytes(EditableComponentCustomData.getActionFor(color));
			};
			heightSlider.OnValueChanged += height => {
				broadcastBytes(EditableComponentCustomData.getActionFor(height));
			};
		}
		
		// Just some cache, cause I don't want to iterate over every component - whenever one is resized.
		// Nor do I want (Like LW) update outlines ever tick -.-
		private HashSet<ComponentAddress> outlineHack;
		
		protected override void OnStartEditing()
		{
			colorPicker.SetColorWithoutNotify(FirstComponentBeingEdited.previousColor);
			heightSlider.SetValueWithoutNotify(FirstComponentBeingEdited.previousHeight);
			
			// Outline update helper stuff - ignore:
			EditableBlockActionHandler.onBlockHeightEdited += outlineMightNeedToBeUpdated;
			outlineHack = new HashSet<ComponentAddress>(ComponentsBeingEdited.Select(e => e.Address));
			
			// Dirty hack to remove the newlines from the component title - only required cause I added newlines...
			gameObject.GetComponent<ConfigurableMenuUtility>().TitleLocalizor.SetLocalizationKey("ComponentActionExampleMod.EditableBlock.NoNewline");
		}
		
		private void outlineMightNeedToBeUpdated(ComponentAddress address)
		{
			if(outlineHack.Contains(address))
			{
				Outliner.RemoveOutline(address);
				Outliner.Outline(address, OutlineData.Select);
			}
		}
		
		protected override void OnClose()
		{
			var newColor = colorPicker.Color24;
			var newHeight = heightSlider.Value;
			
			// The following code is kind of ugly, but well whatever it works...
			var undoRequests = new List<BuildRequest>();
			foreach(var component in ComponentsBeingEdited)
			{
				if (newColor != component.previousColor)
				{
					undoRequests.Add(new BuildRequest_SendComponentAction(
						component.Address,
						EditableComponentCustomData.getActionFor(component.previousColor)
					));
				}
				if (newHeight != component.previousHeight)
				{
					undoRequests.Add(new BuildRequest_SendComponentAction(
						component.Address,
						EditableComponentCustomData.getActionFor(component.previousHeight)
					));
				}
			}
			// Inject collected undo requests into the history:
			if(undoRequests.Count != 0)
			{
				UndoManager.AddItemToUndoHistory(new UndoRequests()
				{
					RequestsToUndo = undoRequests,
				});
			}
			
			// Outline update helper stuff - ignore:
			EditableBlockActionHandler.onBlockHeightEdited += outlineMightNeedToBeUpdated;
			outlineHack.Clear();
			outlineHack = null;
		}
		
		private void broadcastBytes(byte[] bytes)
		{
			foreach(var component in ComponentsBeingEdited)
			{
				BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_SendComponentAction(component.Address, bytes));
			}
		}
		
		protected override bool CanEdit(CustomEditingComponentData componentInfo)
		{
			var type = Instances.MainWorld.ComponentTypes.GetTextID(componentInfo.Component.Data.Type);
			return "ComponentActionExampleMod.EditableBlock".Equals(type);
		}
		
		protected override CustomEditingComponentData CreateComponentInfoFor(ComponentAddress cAddress)
		{
			var info = base.CreateComponentInfoFor(cAddress);
			//This class gets executed for every component to probe, thus client code might be something else.
			if(info.ClientCode is EditableBlock clientCode)
			{
				info.previousHeight = clientCode.currentHeight;
				info.previousColor = clientCode.currentColor;
			}
			return info;
		}
	}
}
