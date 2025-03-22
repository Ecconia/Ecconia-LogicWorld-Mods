using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicUI.Layouts.Controllers;

namespace EccsGuiBuilder.client.Layouts.Helper
{
	// Currently there is an issue, with the 'protected int index' variable not being set by intended means (Awake()) method.
	// This helper here allows to bypass that by simply using 'layout.setIndex(index)' instead.
	public static class BuggyLayoutIndexSetter
	{
		private static readonly Action<ListLayoutBase, int> setIndexRaw;
		
		static BuggyLayoutIndexSetter()
		{
			setIndexRaw = Delegator.createFieldSetter<ListLayoutBase, int>(Fields.getPrivate(typeof(ListLayoutBase), "Index"));
		}
		
		public static void setIndex(this GrowGapListLayout layout, int index)
		{
			layout.ElementsUntilGap = index;
			setIndexRaw(layout, index);
		}
		
		public static void setIndex(this GrowElementListLayout layout, int index)
		{
			layout.ElementIndexToGrow = index;
			setIndexRaw(layout, index);
		}
	}
}
