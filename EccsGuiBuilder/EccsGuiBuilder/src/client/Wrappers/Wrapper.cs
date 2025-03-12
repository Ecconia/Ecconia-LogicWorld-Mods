using System;
using System.Collections.Generic;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsLogicWorldAPI.Client.UnityHelper;
using ThisOtherThing.UI.Shapes;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Wrappers
{
	public abstract class Wrapper
	{
		public readonly GameObject gameObject;
		public readonly RectTransform rectTransform;
		
		public readonly List<Wrapper> children = new List<Wrapper>();
		protected readonly List<(string, GameObject, Func<GameObject, GameObject>)> keys = new List<(string, GameObject, Func<GameObject, GameObject>)>();
		
		protected static GameObject defaultResolver(GameObject o) => o;
		
		private bool isLinked;
		
		protected Wrapper(GameObject gameObject)
		{
			this.gameObject = gameObject;
			
			rectTransform = gameObject.GetComponent<RectTransform>();
			//TBI: Inject missing rect instead of reject?
			if(rectTransform == null)
			{
				throw new Exception("GuiBuilder: Provided GameObject had no RectTransform");
			}
		}
		
		public void collectAllInjectionKeys(List<(string, GameObject, Func<GameObject, GameObject>)> list)
		{
			list.AddRange(keys);
			children.ForEach(child => child.collectAllInjectionKeys(list));
		}
		
		protected void setInheritedFlag()
		{
			if(isLinked)
			{
				throw new Exception("Attempted to assign a component Wrapper to more than one parent, this is not supported. [Reason: Each wrapper wraps exactly one GameObject, trying to add that somewhere else will change the parent].");
			}
			isLinked = true;
		}
	}
	
	/// <summary>
	/// Root wrapper span the whole screen and normally contain a Canvas element.
	/// There is no reason to apply any rect transformations or layouts to this. That is task of the child elements.
	/// </summary>
	/// <typeparam name="T">The most specialized Type of this instance</typeparam>
	public abstract class RootWrapper<T> : Wrapper
	where T : RootWrapper<T>
	{
		protected RootWrapper(GameObject gameObject) : base(gameObject)
		{
		}
		
		//### Named key injection system:
		
		public T injectionKey(string key)
		{
			return injectionKey(key, defaultResolver);
		}
		
		public T injectionKey(string key, Func<GameObject, GameObject> resolver)
		{
			keys.Add((key, gameObject, resolver));
			return (T) this;
		}
		
		//### Generic manipulation:
		
		public T add<CT>()
		where CT : Component
		{
			gameObject.AddComponent<CT>();
			return (T) this;
		}
		
		public T addAndConfigure<CT>(Action<CT> configure)
		where CT : Component
		{
			var component = gameObject.AddComponent<CT>();
			configure(component);
			return (T) this;
		}
		
		//### Tree Relationship:
		
		//The most unsafe way on adding something, can be used, when the wrapper system is not needed.
		// Won't link the wrappers, thus prevent generic injection.
		public T add(GameObject gameObject)
		{
			this.gameObject.addChild(gameObject);
			return (T) this;
		}
		
		public T add<Specialized>(Wrapper<Specialized> wrapper)
		where Specialized : Wrapper<Specialized>
		{
			wrapper.setInheritedFlag();
			children.Add(wrapper);
			gameObject.addChild(wrapper.gameObject);
			return (T) this;
		}
		
		public SimpleWrapper addContainer(string name)
		{
			if(string.IsNullOrEmpty(name))
			{
				name = "Container";
			}
			var container = new GameObject(name);
			//Define a basic parent filling rect (to ensure there is one):
			container.AddComponent<RectTransform>();
			
			//Allow the wrapper to be initialized:
			var wrapper = new SimpleWrapper(container)
				.setAlignment(Alignment.TopLeft)
				.setSize(0, 0);
			//Directly link the wrapper-dependency:
			add(wrapper);
			
			return wrapper;
		}
		
		public T addContainer(string name, Action<SimpleWrapper> func)
		{
			func(addContainer(name));
			return (T) this;
		}
		
		//### Direct variable assignment shortcuts:
		
		public T assignTo(out GameObject externalGameObject)
		{
			externalGameObject = gameObject;
			return (T) this;
		}
		
		public T assignTo<CT>(out CT external, bool deep = false)
		{
			external = deep ? gameObject.GetComponentInChildren<CT>() : gameObject.GetComponent<CT>();
			return (T) this;
		}
		
		public T assignToAuto<CT>(out CT external)
		{
			var extract = gameObject.GetComponent<CT>();
			external = extract != null ? extract : gameObject.GetComponentInChildren<CT>();
			return (T) this;
		}
	}
	
	public abstract class Wrapper<T> : RootWrapper<T>
		where T : Wrapper<T>
	{
		protected Wrapper(GameObject gameObject) : base(gameObject)
		{
		}
		
		//### Direct Rect Transform Manipulation:
		
		public T setAlignment(Alignment alignment)
		{
			switch(alignment)
			{
				case Alignment.TopRight:
					rectTransform.anchorMin = new Vector2(1, 1);
					rectTransform.anchorMax = new Vector2(1, 1);
					rectTransform.pivot = new Vector2(1, 1);
					break;
				case Alignment.TopLeft:
					rectTransform.anchorMin = new Vector2(0, 1);
					rectTransform.anchorMax = new Vector2(0, 1);
					rectTransform.pivot = new Vector2(0, 1);
					break;
				case Alignment.BottomRight:
					rectTransform.anchorMin = new Vector2(1, 0);
					rectTransform.anchorMax = new Vector2(1, 0);
					rectTransform.pivot = new Vector2(1, 0);
					break;
				case Alignment.BottomLeft:
					rectTransform.anchorMin = new Vector2(0, 0);
					rectTransform.anchorMax = new Vector2(0, 0);
					rectTransform.pivot = new Vector2(0, 0);
					break;
				case Alignment.Top:
					rectTransform.anchorMin = new Vector2(0, 1);
					rectTransform.anchorMax = new Vector2(1, 1);
					rectTransform.pivot = new Vector2(0.5f, 1);
					break;
				case Alignment.Right:
					rectTransform.anchorMin = new Vector2(1, 0);
					rectTransform.anchorMax = new Vector2(1, 1);
					rectTransform.pivot = new Vector2(1, 0.5f);
					break;
				case Alignment.Bottom:
					rectTransform.anchorMin = new Vector2(0, 0);
					rectTransform.anchorMax = new Vector2(1, 0);
					rectTransform.pivot = new Vector2(0.5f, 0);
					break;
				case Alignment.Left:
					rectTransform.anchorMin = new Vector2(0, 0);
					rectTransform.anchorMax = new Vector2(0, 1);
					rectTransform.pivot = new Vector2(0, 0.5f);
					break;
				case Alignment.Parent:
					rectTransform.anchorMin = new Vector2(0, 0);
					rectTransform.anchorMax = new Vector2(1, 1);
					rectTransform.pivot = new Vector2(0.5f, 0.5f);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Supplied alignment that does not exist.");
			}
			return (T) this;
		}
		
		public T setWidth(int w)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			return (T) this;
		}
		
		public T setHeight(int h)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
			return (T) this;
		}
		
		public T setSize(int w, int h)
		{
			rectTransform.sizeDelta = new Vector2(w, h);
			return (T) this;
		}
		
		//### Quick layout shortcuts:
		
		//TODO: Remove this on next major update, once other mods are updated.
		[Obsolete("Use LayoutExtensions instead")]
		public T vertical(float spacing = 0, RectOffset padding = null, TextAnchor anchor = TextAnchor.UpperLeft, bool expandHorizontal = false, bool expandVertical = false)
		{
			return addAndConfigure<VerticalLayoutGroup>(layout => {
				layout.childForceExpandWidth = expandHorizontal;
				layout.childForceExpandHeight = expandVertical;
				layout.spacing = spacing;
				if(padding != null)
				{
					layout.padding = padding;
				}
				layout.childAlignment = anchor;
			});
		}
		
		//TODO: Remove this on next major update, once other mods are updated.
		[Obsolete("Use LayoutExtensions instead")]
		public T horizontal(float spacing = 0, RectOffset padding = null, TextAnchor anchor = TextAnchor.UpperLeft, bool expandHorizontal = false, bool expandVertical = false)
		{
			return addAndConfigure<HorizontalLayoutGroup>(layout => {
				layout.childForceExpandWidth = expandHorizontal;
				layout.childForceExpandHeight = expandVertical;
				layout.spacing = spacing;
				if(padding != null)
				{
					layout.padding = padding;
				}
				layout.childAlignment = anchor;
			});
		}
		
		public T fixedSize(float x, float y)
		{
			return addAndConfigure<DefinedSizeLayout>(layout => layout.size = new Vector2(x, y));
		}
		
		//### Debug shortcut:
		
		public T addDebugBackground(Color32 color)
		{
			return addContainer("DebugBackground", container => {
				container
					.add<IgnoreLayout>()
					.addAndConfigure<Rectangle>(rect => {
						rect.ShapeProperties.FillColor = color;
					})
					.setAlignment(Alignment.TopLeft)
					.setSize(0, 0);
				container.rectTransform.anchorMin = Vector2.zero;
				container.rectTransform.anchorMax = Vector2.one;
			});
		}
	}
}
