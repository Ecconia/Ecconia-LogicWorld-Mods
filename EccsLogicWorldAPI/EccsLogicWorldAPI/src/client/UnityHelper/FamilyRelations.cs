using UnityEngine;

namespace EccsLogicWorldAPI.Client.UnityHelper
{
	public static class FamilyRelations
	{
		public static void addChild(this GameObject parent, GameObject child, bool worldPositionStays = false)
		{
			child.transform.SetParent(parent.transform, worldPositionStays);
		}
		
		public static void setParent(this GameObject child, GameObject parent, bool worldPositionStays = false)
		{
			child.transform.SetParent(parent.transform, worldPositionStays);
		}
		
		public static GameObject getChild(this GameObject go, int index)
		{
			return go.transform.GetChild(index).gameObject;
		}
		
		public static GameObject cloneWithParent(this GameObject go, GameObject parent)
		{
			var clone = Object.Instantiate(go, parent.transform, false);
			clone.name = go.name; //Restore original name.
			return clone;
		}
		
		public static GameObject clone(this GameObject go)
		{
			var clone = Object.Instantiate(go);
			clone.name = go.name; //Restore original name.
			return clone;
		}
	}
}
