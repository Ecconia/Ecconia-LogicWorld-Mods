using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using LogicUI.MenuParts.Toggles;
using LogicWorld.UI;
using SimulationStopwatch.Shared;
using TMPro;
using UnityEngine;

namespace SimulationStopwatchGui.Client.EditGUI
{
	public class EditSimulationStopwatch : EditComponentMenu<ISimulationStopwatchData>, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("SimulationStopwatchEditSimulationStopwatchWindow")
				.configureContent(content => content
					.layoutVertical(padding: new RectOffset(20, 20, 10, 20)) // Compensate for the top text taking up some "empty" space adding to the padding.
					.addContainer("LineEntry", line => line
						.layoutVerticalInner(spacing: 10) // Only 10px spacing, make the label a bit closer to it's object
						.add(WS.textLine
							.setLocalizationKey("SimulationStopwatch.Gui.SessionName")
							.setFontSize(40f)
						)
						.add(WS.inputField
							.injectionKey(nameof(filePathInputField))
							.fixedSize(1000, 80)
							.setPlaceholderLocalizationKey("SimulationStopwatch.Gui.SessionName.Hint")
							.disableRichText()
						)
					)
					.addContainer("LineEntry", line => line
						.layoutGrowGapHorizontalInner(gapIndex: IndexHelper.nth(1))
						.add(WS.textLine
							.setLocalizationKey("SimulationStopwatch.Gui.PerServerTickDebugging")
							.setFontSize(40f)
						)
						.add(WS.toggle
							.injectionKey(nameof(extraDebugOnServerTick))
						)
					)
				)
				.add<EditSimulationStopwatch>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public TMP_InputField filePathInputField;
		[AssignMe]
		public ToggleSwitch extraDebugOnServerTick;
		
		public override void Initialize()
		{
			base.Initialize();
			
			extraDebugOnServerTick.OnValueChanged += value => {
				foreach(var component in ComponentsBeingEdited)
				{
					component.Data.printDebugEveryServerTick = value;
				}
			};
		}
		
		protected override void OnStartEditing()
		{
			filePathInputField.text = FirstComponentBeingEdited.Data.sessionName;
		}
		
		protected override void OnClose()
		{
			var value = filePathInputField.text.Trim();
			foreach(var component in ComponentsBeingEdited)
			{
				if(!component.Data.sessionName.Equals(value))
				{
					component.Data.sessionName = value;
				}
			}
		}
	}
}
