using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using FileDump.Shared;
using LogicUI.MenuParts;
using LogicWorld.UI;
using TMPro;
using UnityEngine;

namespace FileDumpGui.Client.EditGUI
{
	public class EditFileDump : EditComponentMenu<IFileDumpData>, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("FileDumpEditFileDumpWindow")
				.configureContent(content => content
					.vertical(20f, new RectOffset(20, 20, 20, 20), expandHorizontal: true)
					.add(WS.textLine
						.setLocalizationKey("FileDump.Gui.PegCount")
						.setFontSize(40f)
					)
					.add(WS.slider
						.injectionKey(nameof(valueSlider))
						.setMin(1)
						.setInterval(1)
						.setMax(32)
						.fixedSize(500, 45) //Min width is defined here...
					)
					.add(WS.textLine
						.setLocalizationKey("FileDump.Gui.FileName")
						.setFontSize(40f)
					)
					.add(WS.inputField
						.injectionKey(nameof(filePathInputField))
						.fixedSize(1000, 80)
						.setPlaceholderLocalizationKey("FileDump.Gui.FileName.Hint")
						.disableRichText()
					)
				)
				.add<EditFileDump>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public InputSlider valueSlider;
		[AssignMe]
		public TMP_InputField filePathInputField;
		
		public override void Initialize()
		{
			base.Initialize();
			
			valueSlider.OnValueChangedInt += value => {
				foreach(var component in ComponentsBeingEdited)
				{
					component.Data.pegCount = (uint) value;
				}
			};
		}
		
		protected override void OnStartEditing()
		{
			valueSlider.SetValueWithoutNotify(FirstComponentBeingEdited.Data.pegCount);
			filePathInputField.text = FirstComponentBeingEdited.Data.fileName;
		}
		
		protected override void OnClose()
		{
			if(ComponentsBeingEdited.Count != 1)
			{
				return; //Do not update the name.
			}
			var value = filePathInputField.text.Trim();
			if(!FirstComponentBeingEdited.Data.fileName.Equals(value))
			{
				FirstComponentBeingEdited.Data.fileName = value;
			}
		}
	}
}
