using LogicUI.Palettes;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using UnityEngine;
using UnityEngine.UI;

namespace EccsWindowHelper.Client.Prefabs
{
	public static class SliderPrefab
	{
		public static GameObject constructSlider()
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0, 0.5f);
				rectTransform.anchoredPosition = new Vector2(111.1111f, 0);
				rectTransform.sizeDelta = new Vector2(-111.1111f, 0f);
			}

			gameObject.AddComponent<CanvasRenderer>();

			constructInputSliderBackground(gameObject);
			RectTransform fillRect = constructInputSliderFillArea(gameObject);
			constructInputSliderOutline(gameObject);
			RectTransform handleRect = constructInputSliderHandleSlideArea(gameObject, out Graphic targetGraphic);

			Slider slider = gameObject.AddComponent<Slider>();
			slider.fillRect = fillRect;
			slider.handleRect = handleRect;
			slider.targetGraphic = targetGraphic;
			slider.navigation = new Navigation()
			{
				mode = Navigation.Mode.None,
			};

			gameObject.addPaletteSelectable(PaletteColor.Accent);
			gameObject.addMakeSliderScrollable();

			gameObject.SetActive(true);
			return gameObject;
		}

		private static void constructInputSliderBackground(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Background");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, -18f);
			}

			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFillShadow = false, //Never draw shadow.
			};
			rectangle.RoundedProperties = new RoundedRects.RoundedProperties()
			{
				Type = RoundedRects.RoundedProperties.RoundedType.Uniform,
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UseMaxRadius = true,
			};

			gameObject.addPaletteGraphic(PaletteColor.Tertiary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static RectTransform constructInputSliderFillArea(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Fill Area");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(-13, 0);
				rectTransform.sizeDelta = new Vector2(-26, -18);
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

			RectTransform fillTransform = constructInputSliderFillAreaFill(gameObject);

			Mask mask = gameObject.AddComponent<Mask>();
			mask.showMaskGraphic = false;

			gameObject.SetActive(true);
			gameObject.setParent(parent);

			return fillTransform;
		}

		private static RectTransform constructInputSliderFillAreaFill(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Fill Area Fill");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 0);
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
				ResolutionMode = RoundedRects.RoundedProperties.ResolutionType.Uniform,
				UseMaxRadius = true,
			};

			gameObject.addPaletteGraphic(PaletteColor.AccentModified);

			gameObject.SetActive(true);
			gameObject.setParent(parent);

			return rectTransform;
		}

		private static void constructInputSliderOutline(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Outline");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, -18f);
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

		private static RectTransform constructInputSliderHandleSlideArea(GameObject parent, out Graphic targetGraphic)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Handle Slide Area");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(-38, 0);
			}

			RectTransform handleRect = constructInputSliderHandle(gameObject, out targetGraphic);

			gameObject.SetActive(true);
			gameObject.setParent(parent);

			return handleRect;
		}

		private static RectTransform constructInputSliderHandle(GameObject parent, out Graphic targetGraphic)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Handle");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 0);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}

			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			targetGraphic = rectangle;
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

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

			constructInputSliderHandleOutline(gameObject);

			gameObject.SetActive(true);
			gameObject.setParent(parent);

			return rectTransform;
		}

		private static void constructInputSliderHandleOutline(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Slider Handle Outline");

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

			AspectRatioFitter aspectRatioFitter = gameObject.AddComponent<AspectRatioFitter>();
			aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
	}
}
