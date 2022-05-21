using TMPro;
using UnityEngine;

namespace EccsWindowHelper.Client.Prefabs
{
	public static class WindowSettingsEntryPrefab
	{
		public static GameObject settingsPrefab(GameObject otherContent, int width = 930, int titleWidth = 400, int interactableWidth = 495, int heightModifier = 1)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 0);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 51.9f * heightModifier);
			}
			gameObject.AddComponent<CanvasRenderer>();
			
			//Content:
			constructSettingsTitle(gameObject, titleWidth);
			constructSettingsArea(gameObject, otherContent, interactableWidth, heightModifier);
			
			gameObject.SetActive(true);
			return gameObject;
		}

		private static void constructSettingsTitle(GameObject parent, int width)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry Title");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0.0f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 0f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = WindowHelper.addTMP(gameObject);
			text.horizontalAlignment = HorizontalAlignmentOptions.Right;

			gameObject.addLocalizedTextMesh();

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
		
		private static void constructSettingsArea(GameObject parent, GameObject otherContent, int width, int heightMultiplier)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry Area");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(1, 0.5f * (heightMultiplier - 1) / heightMultiplier);
				rectTransform.anchorMax = new Vector2(1, 0.5f * (heightMultiplier + 1) / heightMultiplier);
				rectTransform.pivot = new Vector2(1.0f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 0f);
			}

			gameObject.addChild(otherContent);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
	}
}
