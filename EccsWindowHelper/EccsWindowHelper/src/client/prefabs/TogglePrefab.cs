using LogicUI.MenuParts.Toggles;
using LogicUI.Palettes;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsWindowHelper.Client.Prefabs
{
	public static class TogglePrefab
	{
		public static GameObject generateToggle()
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Toggle");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0f, 0f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			constructToggleBackground(gameObject, out PaletteGraphic fillPaletteGraphic);
			constructOnIndicator(gameObject, out Graphic onIndicatorGraphic);
			constructHandle(gameObject, out RectTransform handleRect);

			ToggleSwitch toggleSwitch = gameObject.addToggleSwitch(handleRect, fillPaletteGraphic, onIndicatorGraphic, PaletteColor.Quaternary, PaletteColor.Accent);
			toggleSwitch.SetPaletteColor(PaletteColor.Accent);
			toggleSwitch.SetValueWithoutNotify(true);

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
			aspectRatioFitter.aspectRatio = 1.7f;

			gameObject.SetActive(true);
			return gameObject;
		}

		private static void constructToggleBackground(GameObject parent, out PaletteGraphic fillPaletteGraphic)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Toggle Background");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0.1f);
				rectTransform.anchorMax = new Vector2(1, 0.9f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}

			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFillShadow = false,
			};
			rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
			{
				Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UseMaxRadius = true,
			};

			constructFill(gameObject, out fillPaletteGraphic);
			constructOutline(gameObject);

			gameObject.addPaletteGraphic(PaletteColor.Quaternary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructFill(GameObject parent, out PaletteGraphic fillPaletteGraphic)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Toggle Fill");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFillShadow = false,
			};
			rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
			{
				Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UseMaxRadius = true,
			};

			fillPaletteGraphic = gameObject.addPaletteGraphic(PaletteColor.AccentModified);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructOutline(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Toggle Outline");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
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
				UseMaxRadius = true,
			};

			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructOnIndicator(GameObject parent, out Graphic onIndicatorGraphic)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Toggle OnIndicator");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(1, 0.07f);
				rectTransform.anchorMax = new Vector2(1, 0.93f);
				rectTransform.pivot = new Vector2(0, 0.5f);
				rectTransform.anchoredPosition = new Vector2(11.3f, 0);
				rectTransform.sizeDelta = new Vector2(120, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = WindowHelper.addTMP(gameObject);
			onIndicatorGraphic = text;

			gameObject.addLocalizedTextMesh().SetLocalizationKey("LogicUI.ToggleOn");
			gameObject.addPaletteGraphic(PaletteColor.Text_Primary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructHandle(GameObject parent, out RectTransform handleRect)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Toggle Handle");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			handleRect = rectTransform;
			{
				rectTransform.anchorMin = new Vector2(1, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(1, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0.005f, 0);
				rectTransform.sizeDelta = new Vector2(0, -1);
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

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
	}
}
