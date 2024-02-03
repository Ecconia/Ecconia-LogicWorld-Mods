using CustomWirePlacer.Client.CWP;
using EccsGuiBuilder.Client.Layouts.Controller;
using EccsGuiBuilder.Client.Wrappers;
using EccsLogicWorldAPI.Client.AccessHelpers;
using JimmysUnityUtilities;
using LogicUI;
using LogicUI.MenuParts.Toggles;
using LogicUI.MenuTypes;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicUI.Palettes;
using LogicWorld.Input;
using LogicWorld.UI.HelpList;
using TMPro;
using UnityEngine;

namespace CustomWirePlacer.Client.Windows
{
	public class CWPSettingsWindow : ToggleableSingletonMenu<CWPSettingsWindow>
	{
		public static void toggleVisibility()
		{
			ToggleMenu();
		}
		
		public static void setVisible(bool flag)
		{
			if(flag != MenuIsVisible)
			{
				SetMenuVisible(flag);
			}
		}
		
		public static void initOnce()
		{
			OnMenuShown += OverlayActions.getOverlayShownAction();
			OnMenuHidden += OverlayActions.getOverlayHidingAction();
		}
		
		public static void init()
		{
			var window = WS.window("Custom Wire Placer")
				.setLocalizedTitle("CustomWirePlacer.WindowTitle");
			var content = window.contentWrapper
				.vertical(expandHorizontal: true, anchor: TextAnchor.UpperCenter, padding: new RectOffset(20, 20, 10, 10));
			window.add<CWPSettingsWindow>()
				.addOnCloseAction(HideMenu)
				.add<CloseWindowOnKey>()
				.add<HelpOverlayHelper>()
				.build();
				
			var settings = window.gameObject.GetComponent<ConfigurableMenu>().Settings;
			// build() triggers the setup of settings menu. Lets register a callback after that:
			CoroutineUtility.RunAfterOneFrame(() => {
				//Register settings entries:
				foreach(var entry in CWPSettings.collectSettings())
				{
					if(entry is SettingsWindowTitle title)
					{
						content.addContainer("TitleWrapper")
							.vertical(padding: new RectOffset(0, 0, 5, 2), expandHorizontal: true)
							.add(WS.textLine
								.setLocalizationKey(title.title)
								.configureTMP(tmp => {
									tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
									tmp.fontSize = 70;
								})
							);
						continue;
					}
					SettingsWindowSetting setting = (SettingsWindowSetting) entry;
					var line = content.addContainer("LineEntry")
						.addAndConfigure<GapListLayout>(layout => {
							layout.layoutAlignment = RectTransform.Axis.Horizontal;
							layout.childAlignment = TextAnchor.MiddleRight;
							layout.spacing = 20;
							layout.gapSpace = 10;
						})
						.add(WS.textLine
							.setLocalizationKey(setting.key)
						)
						.add(WS.toggle
							.assignToAuto(out ToggleSwitch toggle)
						);
					//Optionally add hover text for more details:
					if(setting.hoverKey != null)
					{
						//Popup width: 700
						line.add(WS.help
							.fixedSize(50, 50)
							.setLocalizationKey(setting.hoverKey)
							.setColor(PaletteColor.Secondary)
						);
					}
					else
					{
						line.add(WS.empty("HelpSpacer")
							.fixedSize(50, 50)
						);
					}
					
					var settingAdapter = settings.AddSecretSetting(setting.key, setting.defaultValue);
					setting.setter(settingAdapter.GetValue());
					
					toggle.OnValueChanged += b => settingAdapter.SetValue(b); //Update toggle state in settings file.
					toggle.OnValueChanged += setting.setter; //Update toggle state in CWP settings.
					toggle.SetValueWithoutNotify(settingAdapter.GetValue()); //Initial state.
					
					settings.OnSettingsReloaded += () => {
						bool newValue = settingAdapter.GetValue();
						if(toggle.Value != newValue) //Only update the values when they changed.
						{
							toggle.SetValueWithoutNotify(newValue); //Update toggle.
							setting.setter(newValue); //Update CWP settings.
						}
					};
				}
			});
		}
	}
	
	public class CloseWindowOnKey : MonoBehaviour
	{
		private void Update()
		{
			//Closes settings window, if the settings window key has been pressed:
			if(CWPTrigger.OpenSettings.DownThisFrame() || UITrigger.Back.DownThisFrame())
			{
				CWPSettingsWindow.HideMenu();
			}
		}
	}
	
	public class HelpOverlayHelper : MonoBehaviour
	{
		private void Update()
		{
			//Allows toggling the visibility of LogicWorlds Help-Overlay.
			if(Trigger.ToggleHelp.DownThisFrame())
			{
				ToggleableSingletonMenu<HelpListMenu>.ToggleMenu();
			}
		}
	}
}
