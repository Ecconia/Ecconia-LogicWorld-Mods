using System;
using System.Linq.Expressions;
using System.Reflection;
using LogicWorld.GameStates;
using LogicWorld.UI.MainMenu;
using LogicWorld.UI.PauseMenuStuff;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EccsWindowHelper.Client
{
	public class MenuBackgroundClosesWindow : MonoBehaviour, IPointerClickHandler
	{
		private Action closeAction;

		public void makeOnlyHideWindow(Type singletonClass)
		{
			var meth = singletonClass.GetMethod("HideMenu", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if(meth == null)
			{
				throw new Exception("The method for hiding singleton windows cannot be found.");
			}
			closeAction = Expression.Lambda<Action>(Expression.Call(meth)).Compile();
		}

		//[Setting_Toggle("MHG.QuickCloseMenus")]
		//^This is how one should have registered a settings. However this only works once, and LW uses this setting internally.
		//So it will be made public and non-static, so that one can add a setting in the WindowSettings window.
		public bool quickCloseMenu { set; get; } = true;

		public void OnPointerClick(PointerEventData eventData)
		{
			if(quickCloseMenu)
			{
				if(closeAction == null)
				{
					//This code is sadly HEAVILY inspired by the original source (copied), but that is marked as internal and has a "bad" name.
					if(MainMenuBase.CurrentlyOnMainMenu)
					{
						GameStateManager.TransitionTo(MainMenuBase.GameStateTextID);
						return;
					}
					if(PauseMenu.CurrentlyInPauseMenu)
					{
						GameStateManager.TransitionTo(PauseMenu.GameStateTextID);
						return;
					}
					GameStateManager.TransitionBackToBuildingState();
				}
				else
				{
					closeAction();
				}
			}
		}

		public static MenuBackgroundClosesWindow addTo(GameObject parent)
		{
			GameObject gameObject = WindowHelper.makeGameObject("ClickableWindowBackground");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			{
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(0, 0);
				rectTransform.anchoredPosition = new Vector2(0, 0);
			}

			gameObject.AddComponent<Rectangle>().color = new Color(0, 0, 0, 0); //Somehow seems to be relevant, else the clicking will not "hit".

			MenuBackgroundClosesWindow ret = gameObject.AddComponent<MenuBackgroundClosesWindow>();
			gameObject.SetActive(true);
			gameObject.setParent(parent);
			return ret;
		}
	}
}
