using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EccsWindowHelper.Client.Prefabs
{
	public static class WindowSettingsEntryPrefab
	{
		public static GameObject settingsPrefab(GameObject otherContent, int width = 930, int titleWidth = 400, int interactableWidth = 495)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(0, 0);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 0);
			}
			gameObject.AddComponent<CanvasRenderer>();

			//Content:
			constructSettingsTitle(gameObject, titleWidth);
			constructSettingsArea(gameObject, otherContent, interactableWidth);

			gameObject.AddComponent<VerSizeFit>().text = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

			gameObject.SetActive(true);
			return gameObject;
		}

		private class VerSizeFit : UIBehaviour, ILayoutSelfController
		{
			private RectTransform m_Rect;

			public TextMeshProUGUI text;

			private RectTransform rectTransform
			{
				get
				{
					if(m_Rect == null)
					{
						m_Rect = GetComponent<RectTransform>();
					}
					return m_Rect;
				}
			}

			protected override void OnEnable()
			{
				base.OnEnable();
				SetDirty();
			}

			protected override void OnDisable()
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				base.OnDisable();
			}

			protected override void OnRectTransformDimensionsChange() => SetDirty();

			private void SetDirty()
			{
				if(IsActive())
				{
					LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				}
			}

			public void SetLayoutHorizontal()
			{
			}

			public void SetLayoutVertical()
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, text.preferredHeight);
			}
		}

		private static void constructSettingsTitle(GameObject parent, int width)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry Title");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0.5f);
				rectTransform.anchorMax = new Vector2(0, 0.5f);
				rectTransform.pivot = new Vector2(0.0f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 0f);
			}
			gameObject.AddComponent<CanvasRenderer>();

			TextMeshProUGUI text = WindowHelper.addTMP(gameObject);
			text.horizontalAlignment = HorizontalAlignmentOptions.Right;
			text.enableAutoSizing = false;
			text.autoSizeTextContainer = false;
			text.fontSize = 38.1f;
			gameObject.addLocalizedTextMesh();

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}

		private static void constructSettingsArea(GameObject parent, GameObject otherContent, int width)
		{
			GameObject gameObject = WindowHelper.makeGameObject("Eccs: Settings Entry Area");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(1, 0.5f);
				rectTransform.anchorMax = new Vector2(1, 0.5f);
				rectTransform.pivot = new Vector2(1.0f, 0.5f);
				rectTransform.anchoredPosition = new Vector2(0, 0);
				rectTransform.sizeDelta = new Vector2(width, 51.9f);
			}

			gameObject.addChild(otherContent);

			gameObject.SetActive(true);
			gameObject.setParent(parent);
		}
	}
}
