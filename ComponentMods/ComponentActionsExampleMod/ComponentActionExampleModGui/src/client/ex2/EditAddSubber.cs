using System.Collections.Generic;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using LogicAPI.Data.BuildingRequests;
using LogicUI.MenuParts;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicWorld.BuildingManagement;
using LogicWorld.UI;

namespace ComponentActionExampleModGui.Client.Ex2
{
	public class EditAddSubber : EditComponentMenu, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("ComponentActionExampleModGui.AddSubber")
				.setYPosition(400)
				.configureContent(content => content
					.layoutVertical()
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex2.AddSubber.Add")
						.injectionKey(nameof(addButton))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex2.AddSubber.Sub")
						.injectionKey(nameof(subButton))
					)
				)
				.add<EditAddSubber>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public HoverButton addButton;
		
		[AssignMe]
		public HoverButton subButton;
		
		public override void Initialize()
		{
			base.Initialize();
			
			addButton.OnClickEnd += () => {
				updateMode(0);
			};
			subButton.OnClickEnd += () => {
				updateMode(1);
			};
		}
		
		protected override void OnStartEditing()
		{
			// Dirty hack to remove the newlines from the component title - only required cause I added newlines...
			gameObject.GetComponent<ConfigurableMenuUtility>().TitleLocalizor.SetLocalizationKey("ComponentActionExampleMod.AddSubber.NoNewline");
		}
		
		private void updateMode(byte mode)
		{
			foreach(var component in ComponentsBeingEdited)
			{
				BuildRequestManager.SendBuildRequest(new BuildRequest_SendComponentAction(
					component.Address,
					new byte[] {mode}
				));
			}
		}
		
		protected override IEnumerable<string> GetTextIDsOfComponentTypesThatCanBeEdited()
		{
			return new string[]
			{
				"ComponentActionExampleMod.AddSubber",
			};
		}
	}
}
