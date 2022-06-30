using EccsWindowHelper.Client;
using ThisOtherThing.UI.Shapes;
using UnityEngine;

namespace CustomWirePlacer.Client.Windows
{
	public static class CWPUIHelper
	{
		public static void addBackground(GameObject window)
		{
			GameObject backgroundObject = WindowHelper.makeGameObject("CWP: Window Background");
			RectTransform rectTransform = backgroundObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0, 0);
			rectTransform.anchorMax = new Vector2(1, 1);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.sizeDelta = new Vector2(0, 0);

			backgroundObject.AddComponent<CanvasRenderer>();

			Rectangle background = backgroundObject.AddComponent<Rectangle>();
			background.color = new Color(0f, 0f, 0f, 0.5f);

			backgroundObject.SetActive(true);
			backgroundObject.setParent(window);
		}
	}
}
