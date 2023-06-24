using EccsLogicWorldAPI.Client.UnityHelper;
using EccsLogicWorldAPI.Shared.AccessHelper;
using JimmysUnityUtilities;
using LogicUI.Enhancements;
using LogicUI.InputFieldStuff;
using LogicUI.Palettes;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Components
{
	public static class CustomStore
	{
		private static GameObject stockInputField;
		
		public static GameObject genInputField => stockInputField.clone();
		
		public static void init()
		{
			stockInputField = TMP_DefaultControls.CreateInputField(new TMP_DefaultControls.Resources());
			stockInputField.SetActive(false);
			stockInputField.name = "GuiBuilder:InputField";
			var inputField = stockInputField.GetComponent<TMP_InputField>();
			//Fix background image:
			stockInputField.RemoveComponentImmediate<Image>();
			var rect = stockInputField.AddComponent<Rectangle>();
			rect.ShapeProperties.DrawOutline = true;
			rect.OutlineProperties.Type = GeoUtils.OutlineProperties.LineType.Outer;
			rect.ShapeProperties.OutlineColor = new Color32(0, 0, 0, 255);
			rect.RoundedProperties.Type = RoundedRects.RoundedProperties.RoundedType.Uniform;
			rect.RoundedProperties.UniformRadius = 10;
			//Fix input field text:
			var text = stockInputField.getChild(0).getChild(1).GetComponent<TextMeshProUGUI>();
			text.enableAutoSizing = true;
			text.verticalAlignment = VerticalAlignmentOptions.Middle;
			text.extraPadding = false;
			//Fix placeholder:
			var placeholder = stockInputField.getChild(0).getChild(0).GetComponent<TextMeshProUGUI>();
			placeholder.enableAutoSizing = true;
			placeholder.verticalAlignment = VerticalAlignmentOptions.Middle;
			placeholder.extraPadding = false;
			
			//Border:
			var outline = stockInputField.AddComponent<PaletteRectangleOutline>();
			Fields.getPrivate(outline, "Target").SetValue(outline, rect); //Do injection that normally Unity would do on copying...
			outline.SetPaletteColor(PaletteColor.Tertiary);
			outline.SetPaletteOpacity(255);
			//Selection:
			var selection = stockInputField.AddComponent<PaletteInputFieldSelection>();
			Fields.getPrivate(selection, "Target").SetValue(selection, inputField);
			selection.SetPaletteColor(PaletteColor.Accent);
			selection.SetPaletteOpacity(165);
			//Background:
			var background = stockInputField.AddComponent<PaletteGraphic>();
			Fields.getPrivate(background, "Target").SetValue(background, rect);
			background.SetPaletteColor(PaletteColor.InputField);
			background.SetPaletteOpacity(255);
			//Placeholder-Color:
			var placeholderColor = placeholder.AddComponent<PaletteGraphic>();
			Fields.getPrivate(placeholderColor, "Target").SetValue(placeholderColor, placeholder);
			placeholderColor.SetPaletteColor(PaletteColor.InputFieldText);
			placeholderColor.SetPaletteOpacity(127);
			//Text-Color:
			var textColor = text.AddComponent<PaletteGraphic>();
			Fields.getPrivate(textColor, "Target").SetValue(textColor, text);
			textColor.SetPaletteColor(PaletteColor.InputFieldText);
			textColor.SetPaletteOpacity(255);
			//Support for Unicode injection:
			var unicodeInjectionThing = inputField.AddComponent<InputFieldUnicodeShortcut>();
			Fields.getPrivate(unicodeInjectionThing, "thisInput").SetValue(unicodeInjectionThing, inputField);
			//Caret color fixer:
			var caretManager = inputField.AddComponent<InputFieldSettingsApplier>();
			Fields.getPrivate(caretManager, "Target").SetValue(caretManager, inputField);
			
			//Finalize:
			stockInputField.SetActive(true);
			VanillaStore._addInternal(stockInputField);
		}
	}
}
