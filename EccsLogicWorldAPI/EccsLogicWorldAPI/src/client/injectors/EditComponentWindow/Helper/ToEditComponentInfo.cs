using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Interfaces;

namespace EccsLogicWorldAPI.Client.Injectors.EditComponentWindow.Helper
{
	public class ToEditComponentInfo
	{
		public ComponentAddress Address;
		public IComponentInWorld Component;
		public IComponentClientCode ClientCode;
		
		//TBI: Leave public, could be improved - should only be used "internally" anyway.
		public byte[] BeforeEditingCustomData;
		
		public bool hasCustomDataChanged()
		{
			return !Component.Data.CustomData.HasTheSameContentsAs(BeforeEditingCustomData);
		}
		
		public void setBeforeEditingCustomData()
		{
			BeforeEditingCustomData = Component.Data.CustomData.Duplicate();
		}
	}
	
	public class ToEditComponentInfoWithData<DataType> : ToEditComponentInfo
		where DataType : class
	{
		public DataType Data;
		
		public void setCustomDataObject()
		{
			Data = ClientCode?.CustomDataObject as DataType;
		}
	}
}
