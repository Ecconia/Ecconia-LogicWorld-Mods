using EccsWindowHelper.Client;
using TMPro;
using UnityEngine;

namespace CustomWirePlacer.Client.Windows
{
	public class TextOverlayBox
	{
		private GameObject windowObject;
		private RectTransform rect;
		private TextMeshProUGUI text;

		public TextOverlayBox(GameObject parentCanvas, string title)
		{
			windowObject = WindowHelper.makeGameObject(title);
			{
				rect = windowObject.AddComponent<RectTransform>();
				windowObject.AddComponent<CanvasRenderer>();
			}

			CWPUIHelper.addBackground(windowObject);
			{
				GameObject textObject = WindowHelper.makeGameObject("CWP: WindowTextBox: Text");
				RectTransform rectTransform = textObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0, 1);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 1);
				rectTransform.sizeDelta = new Vector2(0, 0);
				textObject.AddComponent<CanvasRenderer>();

				TextMeshProUGUI textMesh = text = textObject.AddComponent<TextMeshProUGUI>();
				textMesh.fontSize = 40;
				textMesh.verticalAlignment = VerticalAlignmentOptions.Top;
				textMesh.horizontalAlignment = HorizontalAlignmentOptions.Left;
				textMesh.autoSizeTextContainer = true;
				textMesh.enableWordWrapping = false;

				textObject.SetActive(true);
				textObject.setParent(windowObject);
			}

			windowObject.SetActive(true);
			windowObject.setParent(parentCanvas);
		}

		public void setText(string text)
		{
			this.text.text = text;
			rect.sizeDelta = new Vector2(this.text.preferredWidth, this.text.preferredHeight);
		}

		public RectTransform getRect()
		{
			return rect;
		}

		public void setActive(bool active)
		{
			windowObject.SetActive(active);
		}
	}
}
