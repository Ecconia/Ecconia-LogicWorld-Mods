using System;
using System.Linq.Expressions;
using System.Reflection;
using EccsWindowHelper.Client.Prefabs;
using LogicLocalization;
using LogicUI;
using LogicUI.MenuParts;
using LogicUI.MenuParts.Toggles;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicUI.Palettes;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using UnityEngine;
using UnityEngine.UI;

namespace EccsWindowHelper.Client
{
	//TODO: Allow creating game-objects in the gameplay scene. Instead of having it loaded all the time.
	public class WindowBuilder
	{
		public int x;
		public int y = 100; //So that the window is centered.
		public int w = 300;
		public int h = 200;
		public bool isResizableX = true;
		public bool isResizableY = true;
		public string titleKey = "";
		public string settingsTitleKey = "LogicUI.ConfigurableMenus.MenuSettings";
		public string rootName; //Mandatory.
		public Type singletonClass; //Mandatory.
		public GameObject contentPlane;
		public Action<GameObject> prepareGameObject;

		public WindowController build()
		{
			GameObject gameObject = WindowHelper.makeOverlayCanvas(rootName);
			gameObject.AddComponent<GraphicRaycaster>();

			prepareGameObject?.Invoke(gameObject);

			GameObject mainWindow = constructWindow(titleKey, contentPlane, out GameObject titleBarFunctionButton, out HoverButton mainWindowCloseButton, out LocalizedTextMesh mainWindowTitle);
			var resizeArrows = addResizePlane(mainWindow);
			// 0Left 1Right 2Down 3Up 4Down/Left 5Down/Right 6Up/Left 7Up/Right 8Move
			//Manually disable resize arrows, since the way that LogicWorld does this, stands in conflict to what I expect. 
			// Nutshell: UP gets never disabled, and the diagonal ones only when X gets disabled.
			if(!isResizableX)
			{
				resizeArrows[0].gameObject.SetActive(false);
				resizeArrows[1].gameObject.SetActive(false);
			}
			if(!isResizableY)
			{
				resizeArrows[2].gameObject.SetActive(false);
				resizeArrows[3].gameObject.SetActive(false);
			}
			if(!isResizableX || !isResizableY)
			{
				resizeArrows[4].gameObject.SetActive(false);
				resizeArrows[5].gameObject.SetActive(false);
				resizeArrows[6].gameObject.SetActive(false);
				resizeArrows[7].gameObject.SetActive(false);
			}
			if(!isResizableX || !isResizableY)
			{
				ContentSizeFitter fitter = mainWindow.AddComponent<ContentSizeFitter>();
				if(!isResizableX)
				{
					fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				}
				if(!isResizableY)
				{
					fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				}
			}
			VerticalLayoutGroup verticalLayoutGroup = mainWindow.AddComponent<VerticalLayoutGroup>();
			verticalLayoutGroup.padding = new RectOffset(10, 10, 80, 10); //The additional 70 is for the top bar.
			verticalLayoutGroup.childForceExpandHeight = false;
			verticalLayoutGroup.childForceExpandWidth = false;
			verticalLayoutGroup.childControlHeight = false;
			verticalLayoutGroup.childControlWidth = false;
			verticalLayoutGroup.spacing = 10;

			mainWindow.SetActive(true);
			gameObject.addChild(mainWindow);

			GameObject settingsWindow = constructMenuSettings(settingsTitleKey);
			gameObject.addChild(settingsWindow);

			ConfigurableMenuSettings confSettings = settingsWindow.GetComponent<ConfigurableMenuSettings>();
			gameObject.addConfigurableMenu(resizeArrows, isResizableX, isResizableY, mainWindow.GetComponent<RectTransform>(), titleBarFunctionButton.GetComponent<ToggleIcon>(), confSettings);

			ConfigurableMenuUtility configurableMenuUtility = gameObject.addConfigurableMenuUtility(mainWindowCloseButton, mainWindowTitle);

			gameObject.addToggleableSingletonMenu(singletonClass);
			var meth = singletonClass.GetMethod("HideMenu", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if(meth == null)
			{
				throw new Exception("The method for hiding singleton windows cannot be found.");
			}
			configurableMenuUtility.OnCloseButtonPressed += Expression.Lambda<Action>(Expression.Call(meth)).Compile();

			return new WindowController(confSettings);
		}

		public static void updateContentPlane(GameObject gameObject)
		{
			//Has to be run, else the window might flicker on first opening.
			GameObject mainWindow = gameObject.transform.parent.gameObject;
			GameObject rootPlane = mainWindow.transform.parent.gameObject;
			rootPlane.SetActive(true);
			LayoutRebuilder.ForceRebuildLayoutImmediate(mainWindow.GetComponent<RectTransform>());
			rootPlane.SetActive(false);
		}

		// ### Windows: ###

		private GameObject constructWindow(string titleKey, GameObject contentPlane, out GameObject titleBarFunctionButton, out HoverButton closeButton, out LocalizedTextMesh localizedTextMesh)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Primary Window");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 1.0f); //It is quite important, that this is the pivot point, else bound check breaks :/
				rectTransform.anchoredPosition = new Vector2(x, y); //Go half the weight up, so that it is centered.
				rectTransform.sizeDelta = new Vector2(w, h);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle windowRect = gameObject.AddComponent<Rectangle>();
			{
				windowRect.ShapeProperties = new GeoUtils.OutlineShapeProperties()
				{
					DrawFillShadow = false, //Never draw shadows.
				};
				windowRect.RoundedProperties = new RoundedRects.RoundedProperties()
				{
					Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
					ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
					UniformRadius = 10,
					UniformResolution = new GeoUtils.RoundingProperties()
					{
						ResolutionMaxDistance = 3,
					},
				};
				windowRect.OutlineProperties = new GeoUtils.OutlineProperties()
				{
					Type = GeoUtils.OutlineProperties.LineType.Outer,
					LineWeight = 4f,
				};
			}

			gameObject.addPaletteGraphic(PaletteColor.Primary);
			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			TitleBar.constructTitleBar(gameObject, false, titleKey, out titleBarFunctionButton, out closeButton, out localizedTextMesh);
			if(contentPlane != null)
			{
				gameObject.addChild(contentPlane);
			}
			constructOutline(gameObject);

			return gameObject;
		}

		private static GameObject constructMenuSettings(string titleKey)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Window");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 1);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 1);
				rectTransform.anchoredPosition = new Vector2(30, -30);
				rectTransform.sizeDelta = new Vector2(950.5f, 0f);
			}

			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFill = true,
				DrawFillShadow = false,
				DrawOutline = true,
				DrawOutlineShadow = false,
			};
			rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
			{
				Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UniformRadius = 10,
				UseMaxRadius = false,
				UniformResolution = new GeoUtils.RoundingProperties()
				{
					ResolutionMaxDistance = 3,
				},
			};
			rectangle.OutlineProperties = new GeoUtils.OutlineProperties()
			{
				Type = GeoUtils.OutlineProperties.LineType.Outer,
				LineWeight = 4f,
			};

			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);
			gameObject.addPaletteGraphic(PaletteColor.Primary);

			//Set layout:
			VerticalLayoutGroup verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
			verticalLayoutGroup.spacing = 10;
			verticalLayoutGroup.childForceExpandHeight = false;
			verticalLayoutGroup.childControlHeight = false;
			verticalLayoutGroup.padding = new RectOffset(10, 10, 80, 10);

			ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
			contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			TitleBar.constructTitleBar(gameObject, true, titleKey, out GameObject titleBarFunctionButton, out HoverButton closeButton, out LocalizedTextMesh _);
			constructOutline(gameObject);

			gameObject.addConfigurableMenuSettings(
				closeButton,
				titleBarFunctionButton.GetComponent<HoverButton>(),
				WindowSettingsEntryPrefab.settingsPrefab(InputSliderPrefab.generateInputSlider()),
				WindowSettingsEntryPrefab.settingsPrefab(TogglePrefab.generateToggle()),
				null
			);

			gameObject.SetActive(true);
			return gameObject;
		}

		// ### Window Parts: ###

		private static void constructOutline(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Window Outline");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.sizeDelta = new Vector2(0, 0);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.raycastTarget = false; //Important, else the content is not clickable.
			{
				rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
				{
					DrawFill = false,
					DrawFillShadow = false,
					DrawOutline = true,
				};
				rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
				{
					Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
					ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				};
				rectangle.OutlineProperties = new GeoUtils.OutlineProperties()
				{
					Type = GeoUtils.OutlineProperties.LineType.Outer,
					LineWeight = 4f,
				};
			}

			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			//Regardless if this object is part of a layout controller, it will be ignored.
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.ignoreLayout = true;

			gameObject.SetActive(true);
			gameObject.transform.SetParent(parent.transform, false);
		}

		private static HoverButton[] addResizePlane(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Window Resize Plane");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}

			//Regardless if this object is part of a layout controller, it will be ignored.
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.ignoreLayout = true;

			var resizeArrows = new HoverButton[9];
			//Left Right Down Up Down/Left Down/Right Up/Left Up/Right Move
			resizeArrows[0] =
				addArrow(0, 0,
				         0, 1,
				         1, 0.5f,
				         15, 0,
				         100, -30,
				         "Eccs: Arrow Left",
				         CursorType.HorizontalArrows);
			resizeArrows[1] =
				addArrow(1, 1,
				         0, 1,
				         0, 0.5f,
				         -15, 0,
				         100, -30,
				         "Eccs: Arrow Right",
				         CursorType.HorizontalArrows);
			resizeArrows[2] =
				addArrow(0, 1,
				         0, 0,
				         0.5f, 1,
				         0, 15,
				         -30, 100,
				         "Eccs: Arrow Down",
				         CursorType.VerticalArrows);
			resizeArrows[3] =
				addArrow(0, 1,
				         1, 1,
				         0.5f, 0,
				         0, -15,
				         -30, 100,
				         "Eccs: Arrow Up",
				         CursorType.VerticalArrows);
			resizeArrows[4] =
				addArrow(0, 0,
				         0, 0,
				         1, 1,
				         15, 15,
				         100, 100,
				         "Eccs: Down Left",
				         CursorType.DiagonalArrowsTopRight);
			resizeArrows[5] =
				addArrow(1, 1,
				         0, 0,
				         0, 1,
				         -15, 15,
				         100, 100,
				         "Eccs: Down Right",
				         CursorType.DiagonalArrowsTopLeft);
			resizeArrows[6] =
				addArrow(0, 0,
				         1, 1,
				         1, 0,
				         15, -15,
				         100, 100,
				         "Eccs: Up Left",
				         CursorType.DiagonalArrowsTopLeft);
			resizeArrows[7] =
				addArrow(1, 1,
				         1, 1,
				         0, 0,
				         -15, -15,
				         100, 100,
				         "Eccs: Up Right",
				         CursorType.DiagonalArrowsTopRight);
			resizeArrows[8] =
				addArrow(0, 1,
				         1, 1,
				         0.5f, 1,
				         -62.5f, -15,
				         -155, 55,
				         "Eccs: Move",
				         CursorType.FourArrows);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return resizeArrows;

			HoverButton addArrow(float xMin, float xMax, float yMin, float yMax, float xP, float yP, float x, float y, float w, float h, string name, CursorType cursor)
			{
				GameObject arrowGameObject = WindowHelper.makeGameObject(name);
				RectTransform rectTransformInner = arrowGameObject.AddComponent<RectTransform>();
				{
					rectTransformInner.anchorMin = new Vector2(xMin, yMin);
					rectTransformInner.anchorMax = new Vector2(xMax, yMax);
					rectTransformInner.pivot = new Vector2(xP, yP);
					rectTransformInner.anchoredPosition = new Vector2(x, y);
					rectTransformInner.sizeDelta = new Vector2(w, h);
				}
				arrowGameObject.AddComponent<CanvasRenderer>();
				arrowGameObject.AddComponent<Image>();
				HoverButton button = arrowGameObject.addHoverButton(null, false, cursor);
				button.SetPaletteOpaque(false);

				arrowGameObject.SetActive(true);
				arrowGameObject.setParent(gameObject);
				return button;
			}
		}
	}
}
