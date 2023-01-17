using LogicLocalization;
using LogicUI.HoverTags;
using LogicUI.MenuParts;
using LogicUI.MenuParts.Toggles;
using LogicUI.Palettes;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This class is not like the other Prefab classes, but it fits quite well in this folder.
namespace EccsWindowHelper.Client.Prefabs
{
	public static class TitleBar
	{
		public static void constructTitleBar(GameObject parent, bool isSettings, string title, out GameObject titleBarFunctionButton, out HoverButton closeButton, out LocalizedTextMesh localizedTextMesh)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 1);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.sizeDelta = new Vector2(0, 70);
				rectTransform.pivot = new Vector2(0.5f, 1.0f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			{
				rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
				{
					DrawFillShadow = false, //Never draw shadows.
					DrawOutline = false, //LW draws an outline here, but there should not be one.
				};
				rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
				{
					Type = RoundedRects.RoundedProperties.RoundedType.Individual,
					ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
					TLRadius = 10,
					TRRadius = 10,
					BRRadius = 0,
					BLRadius = 0,
				};
				rectangle.OutlineProperties = new GeoUtils.OutlineProperties()
				{
					Type = GeoUtils.OutlineProperties.LineType.Outer,
					LineWeight = 4f,
				};
			}
			gameObject.addPaletteGraphic(PaletteColor.Secondary);
			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			//Regardless if this object is part of a layout controller, it will be ignored.
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.ignoreLayout = true;

			constructDividingLine(gameObject);
			constructTitleBarContent(gameObject, isSettings, title, out titleBarFunctionButton, out closeButton, out localizedTextMesh);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructDividingLine(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Dividing Line");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 0);
				rectTransform.sizeDelta = new Vector2(0, 4);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			{
				rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
				{
					DrawFillShadow = false,
				};
				rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
				{
					ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				};
				rectangle.OutlineProperties = new GeoUtils.OutlineProperties()
				{
					Type = GeoUtils.OutlineProperties.LineType.Outer,
					LineWeight = 4f,
				};
			}
			gameObject.addPaletteGraphic(PaletteColor.Tertiary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructTitleBarContent(GameObject parent, bool isSettings, string title, out GameObject titleBarFunctionButton, out HoverButton closeButton, out LocalizedTextMesh localizedTextMesh)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Contents");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.sizeDelta = new Vector2(-25, -20);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(2.5f, 0.0f);
			}

			constructTitle(gameObject, title, out localizedTextMesh);
			titleBarFunctionButton = isSettings ? constructResetSettingsButton(gameObject) : constructShowMenuButton(gameObject);
			closeButton = constructCloseButton(gameObject);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static GameObject constructShowMenuButton(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Show Menu Button");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(1, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(1.0f, 0.5f);
				rectTransform.sizeDelta = new Vector2(50, 0);
				rectTransform.anchoredPosition = new Vector2(-60, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

			HoverTagArea_Localized hoverTag = gameObject.AddComponent<HoverTagArea_Localized>();
			hoverTag.LocalizationKey = "LogicUI.ConfigurableMenus.HoverTags.ShowMenuSettings";

			Graphic graphic = constructShowMenuButtonBackground(gameObject);
			TextMeshProUGUI text = constructShowMenuButtonIcon(gameObject);

			ToggleIcon toggleIcon = gameObject.addToggleIcon("f013", graphic, text);
			toggleIcon.SetValueWithoutNotify(false); //Update the internal state.

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return gameObject;
		}

		private static Graphic constructShowMenuButtonBackground(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Show Menu Button Background");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(0, 0);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFillShadow = false,
				DrawOutline = true,
			};
			rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
			{
				Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UniformRadius = 10f,
				UniformResolution = new GeoUtils.RoundingProperties()
				{
					ResolutionMaxDistance = 3f,
				},
			};
			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return rectangle;
		}

		private static TextMeshProUGUI constructShowMenuButtonIcon(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Show Menu Button Icon");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(-14, -14);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = gameObject.AddComponent<TextMeshProUGUI>();
			text.verticalAlignment = VerticalAlignmentOptions.Middle;
			text.horizontalAlignment = HorizontalAlignmentOptions.Center;
			text.enableAutoSizing = true;
			text.fontSizeMin = 30;
			text.fontSizeMax = 500;
			text.enableWordWrapping = true;
			text.overflowMode = TextOverflowModes.Overflow;
			text.enableKerning = true;
			text.isOrthographic = true;
			gameObject.addPaletteGraphic(PaletteColor.Graphic_Primary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return text;
		}

		private static void constructTitle(GameObject parent, string title, out LocalizedTextMesh localizedTextMesh)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Window Title");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.sizeDelta = new Vector2(-140, 5);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(-70, 2.5f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			WindowHelper.addTMP(gameObject);
			localizedTextMesh = gameObject.addLocalizedTextMesh();
			localizedTextMesh.SetLocalizationKey(title);
			gameObject.addPaletteGraphic(PaletteColor.Text_Primary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static HoverButton constructCloseButton(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Close Button");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(1, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(1.0f, 0.5f);
				rectTransform.sizeDelta = new Vector2(50, 0);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

			Graphic graphic = constructRoundButtonBackground(gameObject);
			constructRoundButtonIcon(gameObject, 12, "f00d");

			HoverButton button = gameObject.addHoverButton(graphic);
			button.SetPaletteColor(PaletteColor.Accent);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return button;
		}

		private static GameObject constructResetSettingsButton(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Reset Settings");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(1, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(1.0f, 0.5f);
				rectTransform.sizeDelta = new Vector2(50, 0);
				rectTransform.anchoredPosition = new Vector2(-60, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

			Graphic graphic = constructRoundButtonBackground(gameObject);
			constructRoundButtonIcon(gameObject, 20, "f0e2");

			HoverTagArea_Localized hoverTag = gameObject.AddComponent<HoverTagArea_Localized>();
			hoverTag.LocalizationKey = "LogicUI.ConfigurableMenus.HoverTags.ResetMenuSettings";

			gameObject.addHoverButton(graphic).SetPaletteColor(PaletteColor.Accent);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return gameObject;
		}

		private static Graphic constructRoundButtonBackground(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Round Button Background");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(0, 0);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFillShadow = false,
				DrawOutline = true,
			};
			rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
			{
				Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UseMaxRadius = true,
			};
			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return rectangle;
		}

		private static void constructRoundButtonIcon(GameObject parent, int padding, string icon)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Title Bar Round Button Icon");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(-padding, -padding);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = gameObject.AddComponent<TextMeshProUGUI>();
			text.verticalAlignment = VerticalAlignmentOptions.Middle;
			text.horizontalAlignment = HorizontalAlignmentOptions.Center;
			text.enableAutoSizing = true;
			text.fontSizeMin = 30;
			text.fontSizeMax = 500;
			text.enableWordWrapping = true;
			text.overflowMode = TextOverflowModes.Overflow;
			text.enableKerning = true;
			text.isOrthographic = true;

			gameObject.addFontIcon(icon).SetPaletteColor(PaletteColor.Graphic_Primary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
	}
}
