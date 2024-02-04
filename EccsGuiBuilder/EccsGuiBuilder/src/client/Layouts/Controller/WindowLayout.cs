using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicUI.MenuTypes.ConfigurableMenus;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Controller
{
	public class WindowLayout : LayoutGroup, ILayoutSelfController
	{
		private static Func<ConfigurableMenu, float> getMinWidth;
		private static Func<ConfigurableMenu, float> getMinHeight;
		private static Action<ConfigurableMenu, float> setMinWidth;
		private static Action<ConfigurableMenu, float> setMinHeight;
		
		static WindowLayout()
		{
			var fieldMinWidth = Fields.getPrivate(typeof(ConfigurableMenu), "MinWidth");
			getMinWidth = Delegator.createFieldGetter<ConfigurableMenu, float>(fieldMinWidth);
			setMinWidth = Delegator.createFieldSetter<ConfigurableMenu, float>(fieldMinWidth);
			var fieldMinHeight = Fields.getPrivate(typeof(ConfigurableMenu), "MinWidth");
			getMinHeight = Delegator.createFieldGetter<ConfigurableMenu, float>(fieldMinHeight);
			setMinHeight = Delegator.createFieldSetter<ConfigurableMenu, float>(fieldMinHeight);
		}
		
		private ConfigurableMenu windowController;
		
		private bool hasUpdated;
		
		protected override void Awake()
		{
			base.Awake();
			windowController = gameObject.transform.parent.GetComponent<ConfigurableMenu>();
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			hasUpdated = false;
		}
		
		private void LateUpdate()
		{
			if(hasUpdated)
			{
				return;
			}
			hasUpdated = true;
			//Force rebuilding, so that LWs min size is set before the first rendering + the min size can be enforced by this layout controller.
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}
		
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal(); //Sets up the components to process.
			
			var miniWidth = 0f;
			var prefWidth = 0f;
			foreach(var child in rectChildren)
			{
				var layout = child.GetComponent<ILayoutElement>();
				if(layout == null)
				{
					continue; //Whoops, this is bad... cannot work with this.
				}
				if(layout.minWidth > miniWidth)
				{
					miniWidth = layout.minWidth;
				}
				var perf = layout.preferredWidth < 0 ? layout.minWidth : layout.preferredWidth;
				if(perf > prefWidth)
				{
					prefWidth = perf;
				}
			}
			var newMinWidth = miniWidth + padding.horizontal;
			if(getMinWidth(windowController) != newMinWidth)
			{
				setMinWidth(windowController, newMinWidth);
			}
			SetLayoutInputForAxis(newMinWidth, prefWidth + padding.horizontal, -1, 0);
		}
		
		public override void CalculateLayoutInputVertical()
		{
			float miniHeight = padding.vertical;
			float prefHeight = padding.vertical;
			foreach(var child in rectChildren)
			{
				var layout = child.GetComponent<ILayoutElement>();
				if(layout == null)
				{
					continue; //Whoops, this is bad... cannot work with this.
				}
				miniHeight += layout.minHeight;
				prefHeight += layout.preferredHeight < 0 ? layout.minHeight : layout.preferredHeight;
			}
			if(getMinHeight(windowController) != miniHeight)
			{
				setMinHeight(windowController, miniHeight);
			}
			SetLayoutInputForAxis(miniHeight, prefHeight, -1, 1);
		}
		
		public override void SetLayoutHorizontal()
		{
			var width = rectTransform.rect.size[0];
			if(width < minWidth)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);
			}
			foreach(var child in rectChildren)
			{
				SetChildAlongAxis(child, 0, 0, width);
			}
		}
		
		public override void SetLayoutVertical()
		{
			float point = padding.top;
			var height = rectTransform.rect.size[1] - padding.bottom;
			if(height < minHeight)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minHeight);
			}
			var useMinSize = height >= GetTotalPreferredSize(1);
			for(var i = rectChildren.Count - 1; i > 0; i--)
			{
				var child = rectChildren[i];
				var layout = child.GetComponent<ILayoutElement>();
				if(layout == null)
				{
					continue; //Whoops, this is bad... cannot work with this.
				}
				var size = layout.preferredHeight < 0 || useMinSize ? layout.minHeight : layout.preferredHeight;
				SetChildAlongAxis(child, 1, point, size);
				point += size;
			}
			if(rectChildren.Count > 0)
			{
				var child = rectChildren[0];
				SetChildAlongAxis(child, 1, point, height - point); //Assign remaining space.
			}
		}
	}
}
