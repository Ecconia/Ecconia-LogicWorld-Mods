using LogicAPI.Data;

namespace EccsLogicWorldAPI.Client.Injectors.EditComponentWindow.Helper
{
	public abstract class EditComponentWindowByData<DataClass> : EditComponentWindowBase<ToEditComponentInfoWithData<DataClass>>
		where DataClass : class
	{
		protected override bool EditingChangesComponentData => true;
		
		protected override ToEditComponentInfoWithData<DataClass> CreateComponentInfoFor(ComponentAddress cAddress)
		{
			var info = base.CreateComponentInfoFor(cAddress);
			info.setCustomDataObject();
			return info;
		}
		
		protected override bool CanEdit(ToEditComponentInfoWithData<DataClass> componentInfo)
		{
			return componentInfo.Data != null;
		}
	}
}
