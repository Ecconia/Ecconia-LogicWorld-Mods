using LogicUI.Palettes;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsWindowHelper.Client.Prefabs
{
	public static class InputSliderPrefab
	{
		public static GameObject generateInputSlider()
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Input Slider");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0.5f);
				rectTransform.anchorMax = new Vector2(1, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0f, 40f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			GameObject slider = SliderPrefab.constructSlider();
			gameObject.addChild(slider);

			constructInputSliderTextField(gameObject);

			gameObject.addInputSlider();

			gameObject.SetActive(true);
			return gameObject;
		}

		private static void constructInputSliderTextField(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Input Slider Text Field");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(92.66f, 0);
			}

			gameObject.AddComponent<CanvasRenderer>();

			Rectangle rectangle = gameObject.AddComponent<Rectangle>();
			rectangle.ShapeProperties = new GeoUtils.OutlineShapeProperties()
			{
				DrawFillShadow = false, //Never draw shadows.
				DrawFill = true,
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

			gameObject.addPaletteGraphic(PaletteColor.InputField);
			gameObject.addPaletteRectangleOutline(PaletteColor.Tertiary);

			constructInputSliderTextFieldTextArea(
				gameObject,
				out RectTransform textViewport,
				out TMP_Text textComponent,
				out Graphic placeholder
			);

			TMP_InputField inputField = gameObject.AddComponent<TMP_InputField>();
			inputField.textViewport = textViewport;
			inputField.textComponent = textComponent;
			inputField.placeholder = placeholder;

			gameObject.addInputFieldSettingsApplier();
			gameObject.addPaletteInputFieldSelection(PaletteColor.Accent, 165);
			gameObject.addMakeInputFieldTabbable();

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructInputSliderTextFieldTextArea(GameObject parent, out RectTransform textViewport, out TMP_Text textComponent, out Graphic placeholder)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Input Slider Text Field Text Area");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			textViewport = rectTransform;
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(-20, 0);
			}

			gameObject.AddComponent<RectMask2D>();

			constructInputSliderTextFieldTextAreaCaret(gameObject);
			constructInputSliderTextFieldTextAreaPlaceholder(gameObject, out placeholder);
			constructInputSliderTextFieldTextAreaText(gameObject, out textComponent);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructInputSliderTextFieldTextAreaCaret(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Input Slider Text Field Text Area Caret");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}

			gameObject.AddComponent<CanvasRenderer>();

			gameObject.AddComponent<TMP_SelectionCaret>();

			gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructInputSliderTextFieldTextAreaPlaceholder(GameObject parent, out Graphic placeholder)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Input Slider Text Field Text Area Placeholder");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}

			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = WindowHelper.addTMP(gameObject);
			placeholder = text;

			gameObject.addPaletteGraphic(PaletteColor.InputFieldText, 127);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructInputSliderTextFieldTextAreaText(GameObject parent, out TMP_Text textComponent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Input Slider Text Field Text Area Text");

			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(0, 0);
			}

			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = WindowHelper.addTMP(gameObject);
			textComponent = text;
			text.horizontalAlignment = HorizontalAlignmentOptions.Center;
			text.enableWordWrapping = false;
			text.fontSizeMin = 18;
			text.fontSizeMax = 72;
			text.enableAutoSizing = true;
			text.richText = false;
			text.margin = new Vector4(0.0f, -5.0f, 0.0f, -5.0f);

			gameObject.addPaletteGraphic(PaletteColor.InputFieldText);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
	}
}
