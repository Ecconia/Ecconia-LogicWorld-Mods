using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Interfaces;

namespace EccsLogicWorldAPI.Client.Injectors.EditComponentWindow.Helper
{
	public abstract class EditComponentWindowByName : EditComponentWindowBase<ToEditComponentInfo>
	{
		private readonly IEnumerable<ComponentType> componentTypesEditable;
		
		protected EditComponentWindowByName(IEnumerable<string> componentTypesEditable)
		{
			//Cache the type for the world. As the world is already loaded, when this gets created.
			this.componentTypesEditable = componentTypesEditable.Select(Instances.MainWorld.ComponentTypes.GetComponentType);
		}
		
		protected override bool CanEdit(ToEditComponentInfo componentInfo)
		{
			return componentTypesEditable.Contains(componentInfo.Component.Data.Type);
		}
	}
}
