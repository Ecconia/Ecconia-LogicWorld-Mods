using System;

namespace EccsGuiBuilder.Client.Wrappers.AutoAssign
{
	[AttributeUsage(AttributeTargets.Field)]
	public class AssignMe : Attribute
	{
		public string key { private set; get; }
		
		public AssignMe(string key)
		{
			this.key = key;
		}
		
		public AssignMe()
		{
			key = null;
		}
	}
}
