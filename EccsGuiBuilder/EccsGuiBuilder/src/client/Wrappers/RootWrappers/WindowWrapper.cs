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
		private event Action onWindowClose;
		
		public WindowWrapper addOnCloseAction(Action action)
		{
			replaceDefaultCloseOperation();
			onWindowClose += action;
			return this;
		}
		
		private void replaceDefaultCloseOperation()
		{
			if(replacedDefaultCloseAction)
			{
				return;
			}
			replacedDefaultCloseAction = true;
			
			var blocker = GameObjectQuery.queryGameObject(gameObject, "Blocker");
			NullChecker.check(blocker, "Cannot find blocker background on custom window canvas");
			
			//Get rid of old code:
			var type = Types.getType(typeof(StandardMenuBackground).Assembly, "LogicWorld.UI.AutomaticCloseButtonBehavior");
			Object.DestroyImmediate(gameObject.GetComponent(type));
			type = Types.getType(typeof(StandardMenuBackground).Assembly, "LogicWorld.UI.ClickableMenuBackground");
			Object.DestroyImmediate(blocker.GetComponent(type)); //Child 0 is the Blocker (background)
			
			//Add close button handler:
			gameObject.GetComponent<ConfigurableMenuUtility>().OnCloseButtonPressed += customOnClose;
			//Add background click handler:
			blocker.AddComponent<CanvasBackgroundClicked>().onCanvasClicked += customOnClose;
		}
		
		private void customOnClose()
		{
			onWindowClose?.Invoke();
		}
		
		//### Resizing and building:
		
		private bool canResizeWidth, canResizeHeight;
		private float minWidth, minHeight;
		
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
			
			//Generic build operation:
			Assigner.assign(this, gameObject); //Run inject framework
			Initializer.recursivelyInitialize(gameObject); //Initialize all Initializables (LWs base class, must be run for some LW, but also custom classes).
		}
	}
}
