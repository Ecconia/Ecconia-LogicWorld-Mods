using System;
using CustomWirePlacer.Client.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPSettingsWindow
	{
		private static CWPSettingsWindow instance;
		
		public static void toggleVisibility()
		{
			instance.rootObject.SetActive(!instance.rootObject.activeSelf);
		}

		public static void setVisible(bool flag)
		{
			instance.rootObject.SetActive(flag);
		}

		public static void Init()
		{
			if(instance != null)
			{
				throw new Exception("CWPSettingsWindow already initialized.");
			}
			instance = new CWPSettingsWindow();
		}

		//Non static:

		private readonly GameObject rootObject;

		private CWPSettingsWindow()
		{
			//Create root component:
			rootObject = new GameObject("CWP-Settings-Window");
			UnityEngine.Object.DontDestroyOnLoad(rootObject);
			rootObject.SetActive(false);
			rootObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
			rootObject.GetComponent<Canvas>().sortingOrder = 32766;
			rootObject.AddComponent<BoxCollider2D>();
			rootObject.AddComponent<ClickableSurface>();
			rootObject.AddComponent<GraphicRaycaster>();
			
			//Create background:
			{
				//Create object:
				GameObject backgroundObject = new GameObject("CWP-Settings-Background");
				UnityEngine.Object.DontDestroyOnLoad(backgroundObject);
				backgroundObject.AddComponent<BoxCollider2D>().size = new Vector2(200, 200);
				backgroundObject.AddComponent<ClickableSurface>();

				//Set content:
				BagkrountDezd background = backgroundObject.AddComponent<BagkrountDezd>();
				background.color = new Color(0.4f, 0.4f, 0.4f, 1f);
				RectTransform transform = background.rectTransform;
				transform.sizeDelta = new Vector2(200f, 200f);
				transform.position = new Vector3(0, 0, 0);
				transform.anchorMin = transform.anchorMax = new Vector2(0.5f, 0.5f);
				transform.pivot = new Vector2(0.5f, 0.5f);
				
				//Link object: (Has to be done after settings the properties)
				backgroundObject.transform.SetParent(rootObject.transform, false);
			}
		}
	}
}
