using FancyInput;
using LogicWorld.Building.Overhaul;
using LogicWorld.GameStates;
using LogicWorld.Interfaces;
using LogicWorld.UI;
using PrimitiveSelections.Client.Inputs;

namespace PrimitiveSelections.Client
{
	public class SelectAllChildComponentOperation : BuildingOperation
	{
		public override string IconHexCode => "e5c2";

		public override bool CanOperateOn(ComponentSelection selection)
		{
			var mainWorldData = Instances.MainWorld.Data;
			foreach(var address in selection)
			{
				if(mainWorldData.Lookup(address).ChildCount != 0)
				{
					return true;
				}
			}
			return false;
		}
		
		public override void BeginOperationOn(ComponentSelection selection)
		{
			var mainWorldData = Instances.MainWorld.Data;
			var newSelection = new ComponentSelection();
			foreach(var address in selection)
			{
				foreach(var childAddress in mainWorldData.Lookup(address).EnumerateChildren())
				{
					newSelection.Add(childAddress);
				}
			}
			
			// This BuildingOperation can be triggered from the MultiSelector GameState (via keybinding).
			// In that case, the game won't clean up MultiSelector when (not) switching to it.
			// To ensure MultiSelector is properly restarted, temporarily switch to the building state - hacky but works.
			// If not done properly the MultiSelector will cause dead outlines.
			GameStateManager.TransitionBackToBuildingState();
			MultiSelector.StartWithSelection(newSelection);
		}
		
		public override InputTrigger OperationStarter => PrimitiveSelectionsTriggers.SelectAllChildComponents;
	}
}
