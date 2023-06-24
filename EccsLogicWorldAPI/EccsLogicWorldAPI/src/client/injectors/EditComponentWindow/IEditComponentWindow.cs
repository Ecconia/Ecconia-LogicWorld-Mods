using System.Collections.Generic;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.Building.Overhaul;

namespace EccsLogicWorldAPI.Client.Injectors.EditComponentWindow
{
	public interface IEditComponentWindow
	{
		/**
		 * Default priority is 0. Your edit window should use 0, like all other editing windows,
		 *  if it determines a component by ID or CustomData type.
		 *
		 * A negative number will mean, it runs after other checks - a fallback - LW uses this for the color-editing-windows.
		 * A number higher than 0 means, you can overshadow existing components and implement your own (e.g. AND gate) edit window.
		 */
		int Priority { get; }
		
		/**
		 * Is called to check if the component at provided address can be edited by this EditWindow.
		 * If a collection is checked, the very first component of that multi-selection will be tested with this function. 
		 */
		bool CanEdit(ComponentAddress cAddress);
		
		/**
		 * Will be called when CanEdit works on the first component of the selection.
		 * The whole selection shall be tested with this function.
		 */
		bool CanEditCollection(IEnumerable<ComponentAddress> collection);
		
		void StartEditing(ComponentSelection selection);
		
		void RunMenu();
		
		void DoCloseActions(out BuildRequest[] undoRequests);
	}
}
