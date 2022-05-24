using CustomWirePlacer.Client.CWP;
using EccsWindowHelper.Client;
using EccsWindowHelper.Client.Prefabs;
using JimmysUnityUtilities;
using LogicLocalization;
using LogicUI.HoverTags;
using LogicUI.MenuParts.Toggles;
using LogicUI.MenuTypes;
using LogicUI.MenuTypes.ConfigurableMenus;
using UnityEngine;
using UnityEngine.UI;

namespace CustomWirePlacer.Client.Windows
{
	public class CWPSettingsWindow : ToggleableSingletonMenu<CWPSettingsWindow>
	{
		private static WindowController controller;

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

		public static void init()
		{
			GameObject contentPlane = constructContent();
			MenuBackgroundClosesWindow menuBackgroundClosesWindow = null;
			WindowBuilder builder = new WindowBuilder
			{
				x = 0,
				y = 100, //Half of height, centers the window. 
				w = 300,
				h = 200,
				isResizableX = false,
				isResizableY = false,
				rootName = "Custom Wire Placer", //Used as file-name by LogicWorld to store settings in.
				titleKey = "CustomWirePlacer.WindowTitle",
				settingsTitleKey = "CustomWirePlacer.WindowSettingsTitle",
				singletonClass = typeof(CWPSettingsWindow),
				contentPlane = contentPlane,
				prepareGameObject = gameObject =>
				{
					//Close window, when pressing the background:
					menuBackgroundClosesWindow = MenuBackgroundClosesWindow.addTo(gameObject);
					menuBackgroundClosesWindow.makeOnlyHideWindow(typeof(CWPSettingsWindow)); //But don't quit the CWP-GameState.
				},
			};
			controller = builder.build();
			Instance.gameObject.AddComponent<CloseWindowOnKey>();
			OnMenuShown += WindowHelper.getOverlayShownAction();
			OnMenuHidden += WindowHelper.getOverlayHidingAction();

			//Settings can only be added after the settings have been initialized, which is scheduled for the next frame.
			CoroutineUtility.RunAfterOneFrame(() =>
			{
				//Add a toggle for closing the window when pressing the background, since it is difficult to get the public setting:
				controller.settingsController.AddSetting_Toggle(new ToggleSettingData()
				{
					SettingKey = "CWP.WindowSetting.CloseWindowOnBackgroundClick",
					DefaultValue = true,
					OnValueUpdated = b => menuBackgroundClosesWindow.quickCloseMenu = b,
				});
				
				//Add all CWP specific settings:
				foreach(var setting in CWPSettings.collectSettings())
				{
					GameObject toggleEntry = WindowSettingsEntryPrefab.settingsPrefab(
						TogglePrefab.generateToggle(),
						1000,
						800,
						200 - 35
					);
					toggleEntry.GetComponentInChildren<LocalizedTextMesh>().SetLocalizationKey(setting.key);
					//Optionally add hover text for more details:
					if(setting.hoverKey != null)
					{
						HoverTagArea_Localized hoverTag = toggleEntry.transform.GetChild(0).gameObject.AddComponent<HoverTagArea_Localized>();
						hoverTag.LocalizationKey = setting.hoverKey;
					}

					//Add secret setting to LWs settings file:
					ConfigurableMenuSettings.SecretSetting<bool> settingAdapter = controller.settingsController.AddSecretSetting(setting.key, setting.defaultValue);
					setting.setter(settingAdapter.GetValue());
					ToggleSwitch toggle = toggleEntry.GetComponentInChildren<ToggleSwitch>();
					toggle.OnValueChanged += b => settingAdapter.SetValue(b); //Update toggle state in settings file.
					toggle.OnValueChanged += setting.setter; //Update toggle state in CWP settings.
					toggle.SetValueWithoutNotify(settingAdapter.GetValue()); //Initial state.
					controller.settingsController.OnSettingsReloaded += () =>
					{
						bool newValue = settingAdapter.GetValue();
						if(toggle.Value != newValue) //Only update the values when they changed.
						{
							toggle.SetValueWithoutNotify(newValue); //Update toggle.
							setting.setter(newValue); //Update CWP settings.
						}
					};

					contentPlane.addChild(toggleEntry);
				}
				WindowBuilder.updateContentPlane(contentPlane); //After content has been added, the window has to be formatted. Else it flickers on first opening.
				// ShowMenu(); //When debugging the window, it is handy to see it on launch.
			});
		}

		private static GameObject constructContent()
		{
			GameObject gameObject = WindowHelper.makeGameObject("CWP: ContentPlane");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(0, 0);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}

			ContentSizeFitter fitter = gameObject.AddComponent<ContentSizeFitter>();
			fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			VerticalLayoutGroup verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
			verticalLayoutGroup.childForceExpandHeight = false;
			verticalLayoutGroup.childControlHeight = false;
			verticalLayoutGroup.childForceExpandWidth = false;
			verticalLayoutGroup.childControlWidth = false;
			verticalLayoutGroup.spacing = 20;

			gameObject.SetActive(true);
			return gameObject;
		}
	}

	public class CloseWindowOnKey : MonoBehaviour
	{
		private void Update()
		{
			//Closes settings window, if the settings window key has been pressed:
			if(CWPTrigger.OpenSettings.DownThisFrame())
			{
				CWPSettingsWindow.HideMenu();
			}
		}
	}
}
