using System;
using System.Linq.Expressions;
using System.Reflection;
using LogicWorld.GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EccsWindowHelper.Client
{
	public static class WindowHelper
	{
		//Two helpers that both establish the relationship of parent and child in pretty.

		public static void addChild(this GameObject parent, GameObject child)
		{
			child.transform.SetParent(parent.transform, false);
		}

		public static void setParent(this GameObject child, GameObject parent)
		{
			child.transform.SetParent(parent.transform, false);
		}

		//Adds or creates parts needed for UI:

		public static TextMeshProUGUI addTMP(GameObject gameObject)
		{
			//Some random default settings, that kind of work in most cases:
			TextMeshProUGUI text = gameObject.AddComponent<TextMeshProUGUI>();
			text.verticalAlignment = VerticalAlignmentOptions.Middle;
			text.horizontalAlignment = HorizontalAlignmentOptions.Left;
			text.enableAutoSizing = true;
			text.fontSizeMin = 30;
			text.fontSizeMax = 500;
			text.enableWordWrapping = true;
			text.enableKerning = true;
			text.isOrthographic = true;
			return text;
		}

		public static GameObject makeGameObject(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.SetActive(false);
			Object.DontDestroyOnLoad(gameObject);
			return gameObject;
		}

		public static GameObject makeOverlayCanvas(string name)
		{
			GameObject gameObject = makeGameObject(name);
			gameObject.AddComponent<RectTransform>();
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(3840, 2160);
			scaler.matchWidthOrHeight = 1;
			return gameObject;
		}

		//These two functions allow access to the game state overlay actions:
		// Uses reflection, since both are set to private.

		public static Action getOverlayShownAction()
		{
			var method = typeof(GameStateManager).GetMethod("OverlayAdded", BindingFlags.Static | BindingFlags.NonPublic);
			if(method == null)
			{
				throw new Exception("Could not find method 'OverlayAdded' in 'GameStateManager'.");
			}
			return Expression.Lambda<Action>(Expression.Call(method)).Compile();
		}

		public static Action getOverlayHidingAction()
		{
			var method = typeof(GameStateManager).GetMethod("OverlayRemoved", BindingFlags.Static | BindingFlags.NonPublic);
			if(method == null)
			{
				throw new Exception("Could not find method 'OverlayRemoved' in 'GameStateManager'.");
			}
			return Expression.Lambda<Action>(Expression.Call(method)).Compile();
		}
	}
}
