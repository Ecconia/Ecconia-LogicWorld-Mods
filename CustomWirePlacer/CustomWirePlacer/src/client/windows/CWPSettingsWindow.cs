using CustomWirePlacer.Client.CWP;
using EccsWindowHelper.Client;
using EccsWindowHelper.Client.Prefabs;
using JimmysUnityUtilities;
using LogicLocalization;
using LogicUI.HoverTags;
using LogicUI.MenuParts.Toggles;
using LogicUI.MenuTypes;
using LogicUI.MenuTypes.ConfigurableMenus;
using TMPro;
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
				y = 550, //Half of height, centers the window roughly. 
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
				foreach(var entry in CWPSettings.collectSettings())
				{
					if(entry is SettingsWindowTitle title)
					{
						injectTitleEntry(contentPlane, title.title, 900);
						continue;
					}
					SettingsWindowSetting setting = (SettingsWindowSetting) entry;
					GameObject toggleEntry = WindowSettingsEntryPrefab.settingsPrefab(
						TogglePrefab.generateToggle(),
						900,
						700,
						200 - 35,
						true
					);
					toggleEntry.GetComponentInChildren<LocalizedTextMesh>().SetLocalizationKey(setting.key);
					//Optionally add hover text for more details:
					if(setting.hoverKey != null)
					{
						injectHoverArea(toggleEntry, setting.hoverKey, 700);
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

		private static void injectTitleEntry(GameObject contentArea, string titleKey, int width)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry Title");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 0);
				rectTransform.pivot = new Vector2(0, 0);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 60f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = WindowHelper.addTMP(gameObject);
			text.enableAutoSizing = false;
			text.autoSizeTextContainer = false;
			text.fontSize = 50f;
			text.verticalAlignment = VerticalAlignmentOptions.Bottom;
			text.horizontalAlignment = HorizontalAlignmentOptions.Center;
			LocalizedTextMesh localizedTextMesh = gameObject.addLocalizedTextMesh();
			localizedTextMesh.SetLocalizationKey(titleKey);

			gameObject.SetActive(true);
			gameObject.setParent(contentArea);
		}

		private static void injectHoverArea(GameObject into, string key, int width)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry Hover Area");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 0f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0);
			gameObject.AddComponent<HoverTagArea_Localized>().LocalizationKey = key;

			gameObject.SetActive(true);
			gameObject.setParent(into);
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
