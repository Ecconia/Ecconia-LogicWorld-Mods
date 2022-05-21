using System;
using System.Reflection;
using LogicLocalization;
using LogicUI;
using LogicUI.Enhancements;
using LogicUI.InputFieldStuff;
using LogicUI.MenuParts;
using LogicUI.MenuParts.Toggles;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicUI.Palettes;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsWindowHelper.Client
{
	public static class InjectionUtils
	{
		private const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static void loadField(out FieldInfo field, Type type, string name)
		{
			field = type.GetField(name, instanceFlags);
			if(field == null)
			{
				throw new Exception("Was not able to find field '" + name + "' in '" + type.Name + "', update/fix mod!");
			}
		}

		// ### Hover Button ###

		private static FieldInfo fieldHoverButtonCursorType;
		private static FieldInfo fieldHoverButtonHoverColors;
		private static FieldInfo fieldHoverButtonClickColors;
		private static FieldInfo fieldHoverButtonTargetGraphic;

		public static HoverButton addHoverButton(this GameObject gameObject, Graphic graphic = null, bool haveColorEffects = true, CursorType cursorType = CursorType.NoModification)
		{
			HoverButton hoverButton = gameObject.AddComponent<HoverButton>();
			if(!haveColorEffects)
			{
				if(fieldHoverButtonHoverColors == null)
				{
					loadField(out fieldHoverButtonHoverColors, typeof(HoverButton), "EnableHoverColors");
					loadField(out fieldHoverButtonClickColors, typeof(HoverButton), "EnableClickColors");
				}
				fieldHoverButtonHoverColors.SetValue(hoverButton, false);
				fieldHoverButtonClickColors.SetValue(hoverButton, false);
			}
			if(cursorType != CursorType.NoModification)
			{
				//Update cursor:
				if(fieldHoverButtonCursorType == null)
				{
					loadField(out fieldHoverButtonCursorType, typeof(HoverButton), "HoveringCursor");
				}
				fieldHoverButtonCursorType.SetValue(hoverButton, cursorType);
			}
			if(fieldHoverButtonTargetGraphic == null)
			{
				loadField(out fieldHoverButtonTargetGraphic, typeof(HoverButton), "TargetGraphic");
			}
			Graphic targetGraphic = graphic == null ? gameObject.GetComponent<Graphic>() : graphic;
			if(targetGraphic == null)
			{
				throw new Exception("Was not able to get a 'Graphic' Component from GameObject. HoverButton requires it though.");
			}
			fieldHoverButtonTargetGraphic.SetValue(hoverButton, targetGraphic);
			return hoverButton;
		}

		// ### Input Slider ###

		private static FieldInfo fieldInputSliderSlider;
		private static FieldInfo fieldInputSliderInputField;

		public static InputSlider addInputSlider(this GameObject gameObject)
		{
			InputSlider inputSlider = gameObject.AddComponent<InputSlider>();
			//Slider:
			Slider slider = gameObject.GetComponentInChildren<Slider>();
			if(slider == null)
			{
				throw new Exception("Cannot get 'Slider' from current GameObject. Mandatory for 'InputSlider'.");
			}
			if(fieldInputSliderSlider == null)
			{
				loadField(out fieldInputSliderSlider, typeof(InputSlider), "Slider");
			}
			fieldInputSliderSlider.SetValue(inputSlider, slider);
			//InputField:
			TMP_InputField inputField = gameObject.GetComponentInChildren<TMP_InputField>();
			if(inputField == null)
			{
				throw new Exception("Cannot get 'TMP_InputField' from current GameObject. Mandatory for 'InputSlider'.");
			}
			if(fieldInputSliderInputField == null)
			{
				loadField(out fieldInputSliderInputField, typeof(InputSlider), "InputField");
			}
			fieldInputSliderInputField.SetValue(inputSlider, inputField);
			//End:
			return inputSlider;
		}

		// ### Palette ###

		//Content:

		private static FieldInfo fieldPaletteGraphicTarget;

		public static PaletteGraphic addPaletteGraphic(this GameObject gameObject, PaletteColor color, byte opacity = 255)
		{
			PaletteGraphic paletteGraphic = gameObject.AddComponent<PaletteGraphic>();
			Graphic graphic = gameObject.GetComponent<Graphic>();
			if(graphic == null)
			{
				throw new Exception("Cannot get 'Graphic' from current GameObject. Mandatory for 'PaletteGraphic'.");
			}
			if(fieldPaletteGraphicTarget == null)
			{
				loadField(out fieldPaletteGraphicTarget, typeof(PaletteGraphic), "Target");
			}
			fieldPaletteGraphicTarget.SetValue(paletteGraphic, graphic);
			paletteGraphic.SetPaletteColor(color);
			paletteGraphic.SetPaletteOpacity(opacity);
			return paletteGraphic;
		}

		//RectangleOutline:

		private static FieldInfo fieldPaletteRectangleOutlineTarget;

		public static PaletteRectangleOutline addPaletteRectangleOutline(this GameObject gameObject, PaletteColor color, byte opacity = 255)
		{
			PaletteRectangleOutline paletteRectangleGraphic = gameObject.AddComponent<PaletteRectangleOutline>();
			Rectangle rectangle = gameObject.GetComponent<Rectangle>();
			if(rectangle == null)
			{
				throw new Exception("Cannot get 'Rectangle' from current GameObject. Mandatory for 'PaletteRectangleOutline'.");
			}
			if(fieldPaletteRectangleOutlineTarget == null)
			{
				loadField(out fieldPaletteRectangleOutlineTarget, typeof(PaletteRectangleOutline), "Target");
			}
			fieldPaletteRectangleOutlineTarget.SetValue(paletteRectangleGraphic, rectangle);
			paletteRectangleGraphic.SetPaletteColor(color);
			paletteRectangleGraphic.SetPaletteOpacity(opacity);
			return paletteRectangleGraphic;
		}

		//InputFieldSelection:

		private static FieldInfo fieldPaletteInputFieldSelectionTarget;

		public static PaletteInputFieldSelection addPaletteInputFieldSelection(this GameObject gameObject, PaletteColor color, byte opacity = 255)
		{
			PaletteInputFieldSelection paletteInputFieldSelection = gameObject.AddComponent<PaletteInputFieldSelection>();
			TMP_InputField tmpInputField = gameObject.GetComponent<TMP_InputField>();
			if(tmpInputField == null)
			{
				throw new Exception("Cannot get 'TMP_InputField' from current GameObject. Mandatory for 'PaletteInputFieldSelection'.");
			}
			if(fieldPaletteInputFieldSelectionTarget == null)
			{
				loadField(out fieldPaletteInputFieldSelectionTarget, typeof(PaletteInputFieldSelection), "Target");
			}
			fieldPaletteInputFieldSelectionTarget.SetValue(paletteInputFieldSelection, tmpInputField);
			paletteInputFieldSelection.SetPaletteColor(color);
			paletteInputFieldSelection.SetPaletteOpacity(opacity);
			return paletteInputFieldSelection;
		}

		//Selectable:

		private static FieldInfo fieldPaletteSelectableTarget;

		public static PaletteSelectable addPaletteSelectable(this GameObject gameObject, PaletteColor color, byte opacity = 255)
		{
			PaletteSelectable paletteSelectable = gameObject.AddComponent<PaletteSelectable>();
			Selectable selectable = gameObject.GetComponent<Selectable>();
			if(selectable == null)
			{
				throw new Exception("Cannot get 'Selectable' from current GameObject. Mandatory for 'PaletteSelectable'.");
			}
			if(fieldPaletteSelectableTarget == null)
			{
				loadField(out fieldPaletteSelectableTarget, typeof(PaletteSelectable), "Target");
			}
			fieldPaletteSelectableTarget.SetValue(paletteSelectable, selectable);
			paletteSelectable.SetPaletteColor(color);
			paletteSelectable.SetPaletteOpacity(opacity);
			return paletteSelectable;
		}

		// ### Localized Text Mesh ###

		private static FieldInfo fieldLocalizedTextMeshTextMesh;

		public static LocalizedTextMesh addLocalizedTextMesh(this GameObject gameObject)
		{
			LocalizedTextMesh localizedTextMesh = gameObject.AddComponent<LocalizedTextMesh>();
			TMP_Text tmpText = gameObject.GetComponent<TMP_Text>();
			if(tmpText == null)
			{
				throw new Exception("Cannot get 'TMP_Text' from current GameObject. Mandatory for 'LocalizedTextMesh'.");
			}
			if(fieldLocalizedTextMeshTextMesh == null)
			{
				loadField(out fieldLocalizedTextMeshTextMesh, typeof(LocalizedTextMesh), "textmesh");
			}
			fieldLocalizedTextMeshTextMesh.SetValue(localizedTextMesh, tmpText);
			return localizedTextMesh;
		}

		// ### Font Icon ###

		private static FieldInfo fieldFontIconIconTextBox;
		private static FieldInfo fieldFontIconIconUnicode;
		private static FieldInfo fieldFontIconIconStyle;

		public static FontIcon addFontIcon(this GameObject gameObject, string unicodeIcon, FontAwesomeStyle style = FontAwesomeStyle.Light)
		{
			FontIcon fontIcon = gameObject.AddComponent<FontIcon>();
			TextMeshProUGUI textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
			if(textMeshPro == null)
			{
				throw new Exception("Cannot get 'TMP_Text' from current GameObject. Mandatory for 'FontIcon'.");
			}
			if(fieldFontIconIconTextBox == null)
			{
				loadField(out fieldFontIconIconTextBox, typeof(FontIcon), "IconTextBox");
			}
			fieldFontIconIconTextBox.SetValue(fontIcon, textMeshPro);
			//Set other fields, this is redundant, but lets keep it consistent.
			if(fieldFontIconIconUnicode == null)
			{
				loadField(out fieldFontIconIconUnicode, typeof(FontIcon), "IconUnicode");
			}
			fieldFontIconIconUnicode.SetValue(fontIcon, unicodeIcon);
			if(fieldFontIconIconStyle == null)
			{
				loadField(out fieldFontIconIconStyle, typeof(FontIcon), "IconStyle");
			}
			fieldFontIconIconStyle.SetValue(fontIcon, style);
			//Initialize, since that does not happen by itself:
			fontIcon.SetIcon(unicodeIcon); //Sets text.
			textMeshPro.font = Fonts.GetFontAwesomeStyle(style); //Sets font.
			return fontIcon;
		}

		// ### Toggle Switch ###

		private static FieldInfo fieldToggleSwitchHandle;
		private static FieldInfo fieldToggleSwitchFill;
		private static FieldInfo fieldToggleSwitchOnIndicator;
		private static FieldInfo fieldToggleSwitchTargetGraphic;
		private static FieldInfo fieldToggleSwitchColorOff;
		private static FieldInfo fieldToggleSwitchColorOn;

		public static ToggleSwitch addToggleSwitch(this GameObject gameObject, RectTransform handleRect, PaletteGraphic fill, Graphic onIndicator, PaletteColor colorOff, PaletteColor colorOn)
		{
			ToggleSwitch toggleSwitch = gameObject.AddComponent<ToggleSwitch>();
			if(fieldToggleSwitchHandle == null)
			{
				loadField(out fieldToggleSwitchHandle, typeof(ToggleSwitch), "Handle");
			}
			fieldToggleSwitchHandle.SetValue(toggleSwitch, handleRect);
			if(fieldToggleSwitchFill == null)
			{
				loadField(out fieldToggleSwitchFill, typeof(ToggleSwitch), "Fill");
			}
			fieldToggleSwitchFill.SetValue(toggleSwitch, fill);
			if(fieldToggleSwitchOnIndicator == null)
			{
				loadField(out fieldToggleSwitchOnIndicator, typeof(ToggleSwitch), "OnIndicator");
			}
			fieldToggleSwitchOnIndicator.SetValue(toggleSwitch, onIndicator);
			if(fieldToggleSwitchTargetGraphic == null)
			{
				loadField(out fieldToggleSwitchTargetGraphic, typeof(ToggleSwitch), "TargetGraphic");
			}
			fieldToggleSwitchTargetGraphic.SetValue(toggleSwitch, handleRect.GetComponent<Graphic>());
			if(fieldToggleSwitchColorOff == null)
			{
				loadField(out fieldToggleSwitchColorOff, typeof(ToggleSwitch), "ColorOff");
			}
			fieldToggleSwitchColorOff.SetValue(toggleSwitch, colorOff);
			if(fieldToggleSwitchColorOn == null)
			{
				loadField(out fieldToggleSwitchColorOn, typeof(ToggleSwitch), "ColorOn");
			}
			fieldToggleSwitchColorOn.SetValue(toggleSwitch, colorOn);
			return toggleSwitch;
		}

		// ### Make Slider Scrollable ###

		private static FieldInfo fieldMakeSliderScrollableThisSlider;

		public static MakeSliderScrollable addMakeSliderScrollable(this GameObject gameObject)
		{
			MakeSliderScrollable localizedTextMesh = gameObject.AddComponent<MakeSliderScrollable>();
			Slider slider = gameObject.GetComponent<Slider>();
			if(slider == null)
			{
				throw new Exception("Cannot get 'Slider' from current GameObject. Mandatory for 'MakeSliderScrollable'.");
			}
			if(fieldMakeSliderScrollableThisSlider == null)
			{
				loadField(out fieldMakeSliderScrollableThisSlider, typeof(MakeSliderScrollable), "thisSlider");
			}
			fieldMakeSliderScrollableThisSlider.SetValue(localizedTextMesh, slider);
			return localizedTextMesh;
		}

		// ### Input Field Settings Applier ###

		private static FieldInfo fieldInputFieldSettingsApplierTarget;

		public static InputFieldSettingsApplier addInputFieldSettingsApplier(this GameObject gameObject)
		{
			InputFieldSettingsApplier inputFieldSettingsApplier = gameObject.AddComponent<InputFieldSettingsApplier>();
			TMP_InputField tmpInputField = gameObject.GetComponent<TMP_InputField>();
			if(tmpInputField == null)
			{
				throw new Exception("Cannot get 'TMP_InputField' from current GameObject. Mandatory for 'InputFieldSettingsApplier'.");
			}
			if(fieldInputFieldSettingsApplierTarget == null)
			{
				loadField(out fieldInputFieldSettingsApplierTarget, typeof(InputFieldSettingsApplier), "Target");
			}
			fieldInputFieldSettingsApplierTarget.SetValue(inputFieldSettingsApplier, tmpInputField);
			return inputFieldSettingsApplier;
		}

		// ### Make Slider Scrollable ###

		private static FieldInfo fieldMakeInputFieldTabbableThisInput;

		public static MakeInputFieldTabbable addMakeInputFieldTabbable(this GameObject gameObject)
		{
			MakeInputFieldTabbable inputFieldSettingsApplier = gameObject.AddComponent<MakeInputFieldTabbable>();
			TMP_InputField tmpInputField = gameObject.GetComponent<TMP_InputField>();
			if(tmpInputField == null)
			{
				throw new Exception("Cannot get 'TMP_InputField' from current GameObject. Mandatory for 'MakeInputFieldTabbable'.");
			}
			if(fieldMakeInputFieldTabbableThisInput == null)
			{
				loadField(out fieldMakeInputFieldTabbableThisInput, typeof(MakeInputFieldTabbable), "thisInput");
			}
			fieldMakeInputFieldTabbableThisInput.SetValue(inputFieldSettingsApplier, tmpInputField);
			return inputFieldSettingsApplier;
		}

		// ### Configuration Menu ###

		private static FieldInfo fieldConfigurableMenuResizeX;
		private static FieldInfo fieldConfigurableMenuResizeY;
		private static FieldInfo[] fieldConfigurableMenuResizeArrows;
		private static FieldInfo fieldConfigurableMenuMainWindow;
		private static FieldInfo fieldConfigurableMenuSettingsToggle;
		private static FieldInfo fieldConfigurableMenuSettings;

		public static ConfigurableMenu addConfigurableMenu(this GameObject gameObject, HoverButton[] resizeArrows, bool resizeX, bool resizeY, RectTransform mainWindowTransform, ToggleIcon settingsToggle, ConfigurableMenuSettings settings)
		{
			ConfigurableMenu configurableMenu = gameObject.AddComponent<ConfigurableMenu>();
			if(fieldConfigurableMenuResizeX == null)
			{
				loadField(out fieldConfigurableMenuResizeX, typeof(ConfigurableMenu), "IsResizableX");
			}
			fieldConfigurableMenuResizeX.SetValue(configurableMenu, resizeX);
			if(fieldConfigurableMenuResizeY == null)
			{
				loadField(out fieldConfigurableMenuResizeY, typeof(ConfigurableMenu), "IsResizableY");
			}
			fieldConfigurableMenuResizeY.SetValue(configurableMenu, resizeY);
			if(fieldConfigurableMenuResizeArrows == null)
			{
				fieldConfigurableMenuResizeArrows = new FieldInfo[9];
				loadField(out fieldConfigurableMenuResizeArrows[0], typeof(ConfigurableMenu), "ResizeLeft");
				loadField(out fieldConfigurableMenuResizeArrows[1], typeof(ConfigurableMenu), "ResizeRight");
				loadField(out fieldConfigurableMenuResizeArrows[2], typeof(ConfigurableMenu), "ResizeDown");
				loadField(out fieldConfigurableMenuResizeArrows[3], typeof(ConfigurableMenu), "ResizeUp");
				loadField(out fieldConfigurableMenuResizeArrows[4], typeof(ConfigurableMenu), "ResizeDownLeft");
				loadField(out fieldConfigurableMenuResizeArrows[5], typeof(ConfigurableMenu), "ResizeDownRight");
				loadField(out fieldConfigurableMenuResizeArrows[6], typeof(ConfigurableMenu), "ResizeUpLeft");
				loadField(out fieldConfigurableMenuResizeArrows[7], typeof(ConfigurableMenu), "ResizeUpRight");
				loadField(out fieldConfigurableMenuResizeArrows[8], typeof(ConfigurableMenu), "ResizeMove");
			}
			fieldConfigurableMenuResizeArrows[0].SetValue(configurableMenu, resizeArrows[0]);
			fieldConfigurableMenuResizeArrows[1].SetValue(configurableMenu, resizeArrows[1]);
			fieldConfigurableMenuResizeArrows[2].SetValue(configurableMenu, resizeArrows[2]);
			fieldConfigurableMenuResizeArrows[3].SetValue(configurableMenu, resizeArrows[3]);
			fieldConfigurableMenuResizeArrows[4].SetValue(configurableMenu, resizeArrows[4]);
			fieldConfigurableMenuResizeArrows[5].SetValue(configurableMenu, resizeArrows[5]);
			fieldConfigurableMenuResizeArrows[6].SetValue(configurableMenu, resizeArrows[6]);
			fieldConfigurableMenuResizeArrows[7].SetValue(configurableMenu, resizeArrows[7]);
			fieldConfigurableMenuResizeArrows[8].SetValue(configurableMenu, resizeArrows[8]);
			if(fieldConfigurableMenuMainWindow == null)
			{
				loadField(out fieldConfigurableMenuMainWindow, typeof(ConfigurableMenu), "Menu");
			}
			fieldConfigurableMenuMainWindow.SetValue(configurableMenu, mainWindowTransform);
			if(fieldConfigurableMenuSettingsToggle == null)
			{
				loadField(out fieldConfigurableMenuSettingsToggle, typeof(ConfigurableMenu), "ShowMenuSettingsToggle");
			}
			fieldConfigurableMenuSettingsToggle.SetValue(configurableMenu, settingsToggle);
			if(fieldConfigurableMenuSettings == null)
			{
				loadField(out fieldConfigurableMenuSettings, typeof(ConfigurableMenu), "<Settings>k__BackingField");
			}
			fieldConfigurableMenuSettings.SetValue(configurableMenu, settings);
			//Also initialize it:
			var initialize = typeof(ConfigurableMenu).GetMethod("LogicWorld.UnityBullshit.IInitializable.Initialize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(initialize == null)
			{
				throw new Exception("Could not invoke 'Initialize' on 'ConfigurableMenu', because this method was not found.");
			}
			initialize.Invoke(configurableMenu, null);
			return configurableMenu;
		}

		// ### Configurable Menu Utility ###

		private static FieldInfo fieldConfigurableMenuUtilityClose;
		private static FieldInfo fieldConfigurableMenuUtilityTitle;

		public static ConfigurableMenuUtility addConfigurableMenuUtility(this GameObject gameObject, HoverButton mainWindowCloseButton, LocalizedTextMesh mainWindowTitle)
		{
			ConfigurableMenuUtility inputFieldSettingsApplier = gameObject.AddComponent<ConfigurableMenuUtility>();
			if(fieldConfigurableMenuUtilityClose == null)
			{
				loadField(out fieldConfigurableMenuUtilityClose, typeof(ConfigurableMenuUtility), "CloseButton");
			}
			fieldConfigurableMenuUtilityClose.SetValue(inputFieldSettingsApplier, mainWindowCloseButton);
			if(fieldConfigurableMenuUtilityTitle == null)
			{
				loadField(out fieldConfigurableMenuUtilityTitle, typeof(ConfigurableMenuUtility), "Title");
			}
			fieldConfigurableMenuUtilityTitle.SetValue(inputFieldSettingsApplier, mainWindowTitle);
			return inputFieldSettingsApplier;
		}

		// ### Toggleable Singleton Menu ###

		public static object addToggleableSingletonMenu(this GameObject gameObject, Type type)
		{
			object tog = gameObject.AddComponent(type);
			var initialize = tog.GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(initialize == null)
			{
				throw new Exception("Could not invoke 'Initialize' on 'ToggleableSingletonMenu' (" + type + "), because this method was not found.");
			}
			initialize.Invoke(tog, null);
			return tog;
		}

		// ### Configuration Menu ###

		private static FieldInfo fieldConfigurableMenuSettingsCloseMenuSettingsButton;
		private static FieldInfo fieldConfigurableMenuSettingsResetSettingsButton;
		private static FieldInfo fieldConfigurableMenuSettingsSettingPrefab_Slider;
		private static FieldInfo fieldConfigurableMenuSettingsSettingPrefab_Toggle;
		private static FieldInfo fieldConfigurableMenuSettingsSettingPrefab_FullWidthButton;

		public static ConfigurableMenuSettings addConfigurableMenuSettings(
			this GameObject gameObject,
			HoverButton closeSettingsButton,
			HoverButton resetSettingsButton,
			GameObject prefabSlider,
			GameObject prefabToggle,
			GameObject prefabButton
		)
		{
			ConfigurableMenuSettings configurableMenu = gameObject.AddComponent<ConfigurableMenuSettings>();
			if(fieldConfigurableMenuSettingsCloseMenuSettingsButton == null)
			{
				loadField(out fieldConfigurableMenuSettingsCloseMenuSettingsButton, typeof(ConfigurableMenuSettings), "CloseMenuSettingsButton");
			}
			fieldConfigurableMenuSettingsCloseMenuSettingsButton.SetValue(configurableMenu, closeSettingsButton);
			if(fieldConfigurableMenuSettingsResetSettingsButton == null)
			{
				loadField(out fieldConfigurableMenuSettingsResetSettingsButton, typeof(ConfigurableMenuSettings), "ResetSettingsButton");
			}
			fieldConfigurableMenuSettingsResetSettingsButton.SetValue(configurableMenu, resetSettingsButton);
			if(fieldConfigurableMenuSettingsSettingPrefab_Slider == null)
			{
				loadField(out fieldConfigurableMenuSettingsSettingPrefab_Slider, typeof(ConfigurableMenuSettings), "SettingPrefab_Slider");
			}
			fieldConfigurableMenuSettingsSettingPrefab_Slider.SetValue(configurableMenu, prefabSlider);
			if(fieldConfigurableMenuSettingsSettingPrefab_Toggle == null)
			{
				loadField(out fieldConfigurableMenuSettingsSettingPrefab_Toggle, typeof(ConfigurableMenuSettings), "SettingPrefab_Toggle");
			}
			fieldConfigurableMenuSettingsSettingPrefab_Toggle.SetValue(configurableMenu, prefabToggle);
			if(fieldConfigurableMenuSettingsSettingPrefab_FullWidthButton == null)
			{
				loadField(out fieldConfigurableMenuSettingsSettingPrefab_FullWidthButton, typeof(ConfigurableMenuSettings), "SettingPrefab_FullWidthButton");
			}
			fieldConfigurableMenuSettingsSettingPrefab_FullWidthButton.SetValue(configurableMenu, prefabButton);
			return configurableMenu;
		}

		// ### Toggle Icon ###

		private static FieldInfo fieldToggleIconIconStyle;
		private static FieldInfo fieldToggleIconTextMesh;
		private static FieldInfo fieldToggleIconTargetGraphic;

		public static ToggleIcon addToggleIcon(this GameObject gameObject, string unicodeIcon, Graphic targetGraphic, TextMeshProUGUI textMesh, FontAwesomeStyle style = FontAwesomeStyle.Light)
		{
			ToggleIcon toggleIcon = gameObject.AddComponent<ToggleIcon>();

			if(fieldToggleIconIconStyle == null)
			{
				loadField(out fieldToggleIconIconStyle, typeof(ToggleIcon), "IconStyle");
			}
			fieldToggleIconIconStyle.SetValue(toggleIcon, style);
			if(fieldToggleIconTextMesh == null)
			{
				loadField(out fieldToggleIconTextMesh, typeof(ToggleIcon), "TextMesh");
			}
			fieldToggleIconTextMesh.SetValue(toggleIcon, textMesh);
			if(fieldToggleIconTargetGraphic == null)
			{
				loadField(out fieldToggleIconTargetGraphic, typeof(ToggleIcon), "TargetGraphic");
			}
			fieldToggleIconTargetGraphic.SetValue(toggleIcon, targetGraphic);
			toggleIcon.SetIconUnicodeDirect(unicodeIcon);
			return toggleIcon;
		}
	}
}
