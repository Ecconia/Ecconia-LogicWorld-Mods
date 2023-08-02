using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicWorld.Building.Overhaul;
using LogicWorld.GameStates;
using LogicWorld.UI;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	public static class Selection
	{
		public static readonly Func<ComponentSelection> getCurrentSelection;
		
		static Selection()
		{
			getCurrentSelection = Delegator.createStaticFieldGetter<ComponentSelection>(
				Fields.getPrivateStatic(typeof(MultiSelector), "CurrentSelection")
			);
		}
		
		public static bool isMultiSelecting()
		{
			return MultiSelector.GameStateTextID.Equals(GameStateManager.CurrentStateID);
		}
	}
}
