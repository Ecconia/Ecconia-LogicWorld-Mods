using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicUI.MenuTypes.ConfigurableMenus;
using UnityEngine;

namespace EccsGuiBuilder.Client
{
	public static class ConfigMenuRectGetter
	{
		private static readonly Func<ConfigurableMenu, RectTransform> getRectTransform;
		
		static ConfigMenuRectGetter()
		{
			getRectTransform = Delegator.createFieldGetter<ConfigurableMenu, RectTransform>(Fields.getPrivate(typeof(ConfigurableMenu), "Menu"));
		}
		
		public static RectTransform getMenuRectTransform(this ConfigurableMenu menu)
		{
			return getRectTransform(menu);
		}
	}
}
