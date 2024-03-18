using System.Collections.Generic;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using LogicAPI.Data.BuildingRequests;
using LogicUI.MenuParts;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicWorld.BuildingManagement;
using LogicWorld.UI;
using UnityEngine;

namespace ComponentActionExampleModGui.Client.Ex3
{
	public class EditPulseByEdit : EditComponentMenu, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("ComponentActionExampleModGui.PulseByEdit")
				.configureContent(content => content
					.vertical(20f, new RectOffset(20, 20, 20, 20), expandHorizontal: true)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex3.PulseByEdit.P1")
						.injectionKey(nameof(pulseButton1))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex3.PulseByEdit.P2")
						.injectionKey(nameof(pulseButton2))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex3.PulseByEdit.P3")
						.injectionKey(nameof(pulseButton3))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex3.PulseByEdit.P4")
						.injectionKey(nameof(pulseButton4))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex3.PulseByEdit.P5")
						.injectionKey(nameof(pulseButton5))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.setLocalizationKey("ComponentActionExampleMod.Gui.Ex3.PulseByEdit.P6")
						.injectionKey(nameof(pulseButton6))
					)
				)
				.add<EditPulseByEdit>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public HoverButton pulseButton1;
		[AssignMe]
		public HoverButton pulseButton2;
		[AssignMe]
		public HoverButton pulseButton3;
		[AssignMe]
		public HoverButton pulseButton4;
		[AssignMe]
		public HoverButton pulseButton5;
		[AssignMe]
		public HoverButton pulseButton6;
		
		public override void Initialize()
		{
			base.Initialize();
			
			pulseButton1.OnClickEnd += () => {
				sendPulse(1);
			};
			pulseButton2.OnClickEnd += () => {
				sendPulse(2);
			};
			pulseButton3.OnClickEnd += () => {
				sendPulse(3);
			};
			pulseButton4.OnClickEnd += () => {
				sendPulse(4);
			};
			pulseButton5.OnClickEnd += () => {
				sendPulse(5);
			};
			pulseButton6.OnClickEnd += () => {
				sendPulse(6);
			};
		}
		
		protected override void OnStartEditing()
		{
			// Dirty hack to remove the newlines from the component title - only required cause I added newlines...
			gameObject.GetComponent<ConfigurableMenuUtility>().TitleLocalizor.SetLocalizationKey("ComponentActionExampleMod.PulseByEdit.NoNewline");
		}
		
		private void sendPulse(byte pulseLength)
		{
			foreach(var component in ComponentsBeingEdited)
			{
				BuildRequestManager.SendBuildRequest(new BuildRequest_SendComponentAction(
					component.Address,
					new byte[] {pulseLength}
				));
			}
		}
		
		protected override IEnumerable<string> GetTextIDsOfComponentTypesThatCanBeEdited()
		{
			return new string[]
			{
				"ComponentActionExampleMod.PulseByEdit",
			};
		}
	}
}
