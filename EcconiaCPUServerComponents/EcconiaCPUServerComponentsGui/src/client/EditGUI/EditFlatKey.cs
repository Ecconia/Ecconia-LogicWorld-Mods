using EcconiaCPUServerComponents.Shared;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using FancyInput;
using LogicLocalization;
using LogicUI.ColorChoosing;
using LogicUI.MenuParts;
using LogicWorld.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EcconiaCPUServerComponentsGui.Client.EditGUI
{
	public class EditFlatKey : EditComponentMenu<IFlatKeyData>, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("EccComponentsEditFlatKeyWindow")
				.setYPosition(870)
				.configureContent(content => content
					.layoutVertical()
					.addContainer("TopBarBox", topBar => topBar
						.layoutHorizontalInner()
						.add(WS.textLine.setLocalizationKey("EcconiaCPUServerComponents.Gui.FlatKey.BackgroundLabel"))
						.add(WS.colorPicker
							.injectionKey(nameof(backgroundColorPicker))
							.fixedSize(210, 70)
						)
						.add(WS.textLine.setLocalizationKey("EcconiaCPUServerComponents.Gui.FlatKey.ForegroundLabel"))
						.add(WS.colorPicker
							.injectionKey(nameof(foregroundColorPicker))
							.fixedSize(210, 70)
						)
						.add(WS.button
							.setLocalizationKey("EcconiaCPUServerComponents.Gui.FlatKey.EditKeybinding")
							.injectionKey(nameof(editKeyBindButton))
							.add<ButtonLayout>()
						)
					)
					.addContainer("MainContentBox", mainContent => mainContent
						.layoutGrowElementHorizontalInner(elementIndex: IndexHelper.nth(1))
						.add(WS.keyHighlighter
							.injectionKey("keyHighlight")
							.fixedSize(500, 500)
						)
						.addContainer("CustomLabelBox", topRightContainer => topRightContainer
							.layoutGrowElementVerticalInner(spacing: 10, elementIndex: IndexHelper.nth(1))
							.add(WS.textLine.setLocalizationKey("EcconiaCPUServerComponents.Gui.FlatKey.CustomLabel"))
							.add(WS.inputArea
								.injectionKey(nameof(customLabelInput))
								.fixedSize(200, 400)
								.fixTextAreaColor()
								.setPlaceholderLocalizationKey("EcconiaCPUServerComponents.Gui.FlatKey.CustomTextHint")
								.configure(inputField => {
									inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
									var text = inputField.textComponent;
									text.textWrappingMode = TextWrappingModes.PreserveWhitespace;
									text.fontSize = 60;
									var placeholder = (TMP_Text) inputField.placeholder;
									placeholder.fontSizeMax = 100;
									placeholder.textWrappingMode = TextWrappingModes.PreserveWhitespace;
								})
							)
						)
					)
				)
				.add<EditFlatKey>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public ColorChooser backgroundColorPicker;
		[AssignMe]
		public ColorChooser foregroundColorPicker;
		[AssignMe("keyHighlight")]
		public Graphic keyHighlightBackground;
		[AssignMe("keyHighlight")]
		public TextMeshProUGUI keyHighlightForeground;
		[AssignMe]
		public HoverButton editKeyBindButton;
		[AssignMe]
		public TMP_InputField customLabelInput;
		
		private bool isEditingKeybinding;
		
		public override void Initialize()
		{
			base.Initialize();
			
			//Setup events and handlers:
			editKeyBindButton.OnClickEnd += () => {
				isEditingKeybinding = true;
				keyHighlightForeground.text = TextLocalizer.LocalizeByKey("MHG.UI.KeyMenu.PressAnyButton");
				editKeyBindButton.gameObject.SetActive(true);
				DisableBuiltInKeyboardShortcuts();
			};
			backgroundColorPicker.OnColorChange24 += color => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.KeyColor = color;
				}
				keyHighlightBackground.color = color.WithOpacity();
			};
			foregroundColorPicker.OnColorChange24 += color => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.KeyLabelColor = color;
				}
				keyHighlightForeground.color = color.WithOpacity();
			};
			customLabelInput.onValueChanged.AddListener(label => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.label = label;
				}
			});
		}
		
		protected override void OnStartEditing()
		{
			var data = FirstComponentBeingEdited.Data;
			backgroundColorPicker.SetColorWithoutNotify(data.KeyColor.WithOpacity());
			foregroundColorPicker.SetColorWithoutNotify(data.KeyLabelColor.WithOpacity());
			keyHighlightBackground.color = data.KeyColor.WithOpacity();
			keyHighlightForeground.color = data.KeyLabelColor.WithOpacity();
			keyHighlightForeground.text = ((RawInput) data.BoundInput).DisplayName();
			customLabelInput.text = data.label != null ? data.label : "";
		}
		
		protected override void OnRun()
		{
			if(!isEditingKeybinding)
			{
				return;
			}
			var rawInput = GameInput.WhichInputDownThisFrame();
			if(rawInput == RawInput.None)
			{
				return;
			}
			keyHighlightForeground.text = rawInput.DisplayName();
			editKeyBindButton.gameObject.SetActive(true);
			isEditingKeybinding = false;
			EnableBuiltInKeyboardShortcuts();
			foreach(var entry in ComponentsBeingEdited)
			{
				entry.Data.BoundInput = (int) rawInput;
			}
		}
	}
}
