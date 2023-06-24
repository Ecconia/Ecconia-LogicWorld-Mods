using System;
using EccsLogicWorldAPI.Shared;
using EccsLogicWorldAPI.Shared.AccessHelper;
using JimmysUnityUtilities;
using LogicLocalization;
using LogicUI.Palettes;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Wrappers.Specialized
{
	public class TextFieldWrapper : Wrapper<TextFieldWrapper>
	{
		private readonly TMP_InputField inputField;
		
		public TextFieldWrapper(GameObject gameObject) : base(gameObject)
		{
			inputField = gameObject.GetComponentInChildren<TMP_InputField>();
			NullChecker.check(inputField, "Could not find LocalizedTextMesh inside of GameObject");
		}
		
		public LocalizedTextMesh getPlaceholderLocalizedTextMesh()
		{
			var ltm = inputField.placeholder.GetComponent<LocalizedTextMesh>();
			if(ltm == null)
			{
				ltm = inputField.placeholder.AddComponent<LocalizedTextMesh>();
				Fields.getPrivate(ltm, "textmesh").SetValue(ltm, inputField.placeholder);
			}
			return ltm;
		}
		
		public TextFieldWrapper configure(Action<TMP_InputField> callback)
		{
			callback(inputField);
			return this;
		}
		
		public TextFieldWrapper setPlaceholderText(string text)
		{
			((TMP_Text) inputField.placeholder).text = text;
			return this;
		}
		
		public TextFieldWrapper setPlaceholderLocalizationKey(string text)
		{
			var ltm = getPlaceholderLocalizedTextMesh();
			ltm.SetLocalizationKey(text);
			return this;
		}
		
		public TextFieldWrapper setPlaceholderLocalizationKeyAndParams(string text, params object[] localizationParams)
		{
			var ltm = getPlaceholderLocalizedTextMesh();
			ltm.SetLocalizationKeyAndParams(text, localizationParams);
			return this;
		}
		
		public TextFieldWrapper disableRichText()
		{
			inputField.textComponent.richText = false;
			return this;
		}
		
		public TextFieldWrapper fixTextAreaColor(Color color, Color? colorBG = null)
		{
			if(colorBG == null)
			{
				colorBG = Color.white;
			}
			inputField.textComponent.color = color;
			inputField.GetComponent<Rectangle>().ShapeProperties.FillColor = (Color) colorBG;
			return this;
		}
		
		public TextFieldWrapper fixTextAreaColor()
		{
			//Follow theme:
			var text = inputField.textComponent;
			text.gameObject.SetActive(false);
			var palette = text.gameObject.AddComponent<PaletteGraphic>();
			NullChecker.check(palette, "Whoops palette is null, this is weird... #1");
			var field = Fields.getPrivate(palette, "Target");
			field.SetValue(palette, text);
			palette.SetPaletteColor(PaletteColor.InputFieldText);
			palette.SetPaletteOpacity(255);
			text.gameObject.SetActive(true);
			
			//Idk, just add it to the root as whatever goes wrong there, goes wrong hard...
			gameObject.SetActive(false);
			palette = gameObject.AddComponent<PaletteGraphic>();
			NullChecker.check(palette, "Whoops palette is null, this is weird... #2");
			var graphic = gameObject.GetComponent<Graphic>();
			NullChecker.check(graphic, "Could not find graphic...");
			field.SetValue(palette, graphic);
			palette.SetPaletteColor(PaletteColor.InputField);
			palette.SetPaletteOpacity(255);
			gameObject.SetActive(true);
			return this;
		}
	}
}
