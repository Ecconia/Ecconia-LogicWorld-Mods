using System;
using JetBrains.Annotations;

namespace EccsGuiBuilder.Client.Wrappers.AutoAssign
{
	[MeansImplicitUse(ImplicitUseKindFlags.Assign)]
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
