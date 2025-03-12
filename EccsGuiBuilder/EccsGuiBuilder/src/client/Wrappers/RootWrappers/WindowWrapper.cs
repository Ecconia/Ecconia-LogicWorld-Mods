using System;
using EccsGuiBuilder.Client.CustomBehaviors;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsLogicWorldAPI.Client.AccessHelpers;
using EccsLogicWorldAPI.Client.UnityHelper;
using EccsLogicWorldAPI.Shared;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicWorld.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Types = EccsLogicWorldAPI.Shared.AccessHelper.Types;

namespace EccsGuiBuilder.Client.Wrappers.RootWrappers
{
	/// <summary>
	/// A wrapper for LW provided Window Canvas GameObjects.
	/// </summary>
	public class WindowWrapper : RootWrapper<WindowWrapper>
	{
		public readonly GameObject windowFrame;
		public readonly SimpleWrapper contentWrapper;
		
		public WindowWrapper(GameObject gameObject, string name) : base(gameObject)
		{
			gameObject.name = name;
			windowFrame = GameObjectQuery.queryGameObject(gameObject, "GuiBuilder:WindowFrame");
			NullChecker.check(windowFrame, "Cannot find the window frame inside of window canvas, something went wrong. (Update or developer mess up).");
			contentWrapper = new SimpleWrapper(windowFrame.getChild(0));
			children.Add(contentWrapper); //Not directly true, but needed for traversal. The WindowFrame is skipped as not wrapped.
		}
		
		//As content is already set and won't be added later on, this gives direct in-line access.
		public WindowWrapper configureContent(Action<SimpleWrapper> configurator)
		{
			configurator(contentWrapper);
			return this;
		}
		
		//In case the window is standalone and not a stock EditComponent window.
		public WindowWrapper setLocalizedTitle(string key, params object[] arguments)
		{
			var utilities = gameObject.GetComponent<ConfigurableMenuUtility>();
			utilities.TitleLocalizor.SetLocalizationKeyAndParams(key, arguments);
			return this;
		}
		
		//### Custom close action:
		
		private bool replacedDefaultCloseAction;
		
		public WindowWrapper addOnCloseAction(Action action)
		{
			var blocker = GameObjectQuery.queryGameObject(gameObject, "Blocker");
			NullChecker.check(blocker, "Cannot find blocker background on custom window canvas");
			
			replaceDefaultCloseOperation(blocker);
			
			//Add close button handler:
			gameObject.GetComponent<ConfigurableMenuUtility>().OnCloseButtonPressed += action;
			//Add background click handler:
			blocker.GetComponent<CanvasBackgroundClicked>().onCanvasClicked += action;
			return this;
		}
		
		private void replaceDefaultCloseOperation(GameObject blocker)
		{
			if(replacedDefaultCloseAction)
			{
				return;
			}
			replacedDefaultCloseAction = true;
			
			// The AutomaticCloseButtonBehavior registers an action on the closing button to go back to building game-state on Start(). We do not want that.
			var type = Types.getType(typeof(StandardMenuBackground).Assembly, "LogicWorld.UI.AutomaticCloseButtonBehavior");
			Object.DestroyImmediate(gameObject.GetComponent(type));
			
			// The ClickableMenuBackground will go back to building game-state when the mouse clicks it. We do not want that.
			type = Types.getType(typeof(StandardMenuBackground).Assembly, "LogicWorld.UI.ClickableMenuBackground");
			Object.DestroyImmediate(blocker.GetComponent(type));
			// Instead we want a custom action to be performed, with a custom background click handler:
			blocker.AddComponent<CanvasBackgroundClicked>();
		}
		
		public WindowWrapper doNotBlurBuildingCanvas()
		{
			var background = gameObject.GetComponent<StandardMenuBackground>();
			Fields.getPrivate(background, "BuildingCanvasAffected").SetValue(background, false);
			return this;
		}
		
		//### Resizing and building:
		
		private float defaultWidth, defaultHeight;
		private bool canResizeWidth, canResizeHeight;
		private float minWidth, minHeight;
		private int? targetYPosition = 750;
		
		/// <summary>
		/// Sets the Y position at which the top-border of a window spawns.
		/// </summary>
		/// <param name="targetYPosition">If supplied integer Y position is set. If supplied 'null' the height of the window is used to center the window.</param>
		/// <remarks>The window is centered on the horizontal axis. <b>Warning</b>: For the height to be used in auto centering, a height default/min height must be set!</remarks>
		public WindowWrapper setYPosition(int? targetYPosition)
		{
			this.targetYPosition = targetYPosition;
			return this;
		}
		
		public WindowWrapper setDefaultSize(int width, int height)
		{
			defaultWidth = width;
			defaultHeight = height;
			return this;
		}
		
		public WindowWrapper setResizeable(bool resize = true)
		{
			canResizeWidth = resize;
			canResizeHeight = resize;
			return this;
		}
		
		public WindowWrapper setResizeableHorizontal(bool resize = true)
		{
			canResizeWidth = true;
			return this;
		}
		
		public WindowWrapper setResizeableVertically(bool resize = true)
		{
			canResizeHeight = true;
			return this;
		}
		
		public WindowWrapper setMinSize(float w, float h)
		{
			minWidth = w;
			minHeight = h;
			return this;
		}
		
		public WindowWrapper setMinWidth(float w)
		{
			minWidth = w;
			return this;
		}
		
		public WindowWrapper setMinHeight(float h)
		{
			minHeight = h;
			return this;
		}
		
		public void build()
		{
			var menu = gameObject.GetComponent<ConfigurableMenu>();
			Fields.getPrivate(menu, "IsResizableX").SetValue(menu, canResizeWidth);
			Fields.getPrivate(menu, "IsResizableY").SetValue(menu, canResizeHeight);
			Fields.getPrivate(menu, "MinWidth").SetValue(menu, minWidth);
			Fields.getPrivate(menu, "MinHeight").SetValue(menu, minHeight);
			if(!canResizeWidth || !canResizeHeight)
			{
				var fitter = windowFrame.AddComponent<ContentSizeFitter>();
				if(!canResizeWidth)
				{
					fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
				}
				if(!canResizeHeight)
				{
					fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
				}
			}
			
			var rawMenu = menu.getMenuRectTransform();
			if (canResizeWidth)
			{
				rawMenu.sizeDelta = new Vector2(Mathf.Max(defaultWidth, minWidth), rawMenu.sizeDelta.y);
			}
			if (canResizeHeight)
			{
				rawMenu.sizeDelta = new Vector2(rawMenu.sizeDelta.x, Mathf.Max(defaultHeight, minHeight));
			}
			
			//Set Y-Position of window or automatically center the window based on targetYPosition:
			rawMenu.anchoredPosition = new Vector2(0, targetYPosition ?? rawMenu.sizeDelta.y / 2);
			
			//Generic build operation:
			Assigner.assign(this, gameObject); //Run inject framework
			Initializer.recursivelyInitialize(gameObject); //Initialize all Initializables (LWs base class, must be run for some LW, but also custom classes).
		}
	}
}
