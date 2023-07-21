using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicWorld.UI;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	public static class EditWindowRefresher
	{
		private static readonly Func<object> getCurrentlyOpenWindow;
		private static readonly Action<ComponentAddress> callChangeMethod;
		
		static EditWindowRefresher()
		{
			var componentMenuManager = Types.findInAssembly(typeof(EditComponentMenu), "LogicWorld.UI.ComponentMenusManager");
			getCurrentlyOpenWindow = Delegator.createStaticFieldGetter<object>(Fields.getPrivateStatic(componentMenuManager, "CurrentlyOpenMenu"));
			callChangeMethod = Delegator.createStaticMethodCall<ComponentAddress>(Methods.getPrivateStatic(componentMenuManager, "OnComponentPegCountChanged"));
		}
		
		/*
		 * Call this function from a component if its custom data has changed and thus its outer appearance (mostly outline).
		 *  Do not call this when peg count has changed, that already updates automatically.
		 */
		public static void updateEditWindow(ComponentAddress address)
		{
			if(getCurrentlyOpenWindow() == null)
			{
				return; //No open edit window, skip.
			}
			callChangeMethod(address);
		}
	}
}
