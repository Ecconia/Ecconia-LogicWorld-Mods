using System;
using System.Linq;
using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	public static class Assemblies
	{
		public static Assembly findAssemblyWithName(string name)
		{
			var harmonyAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(ass => name.Equals(ass.GetName().Name));
			if(harmonyAssembly == null)
			{
				throw new AccessHelperException("Could not find '" + name + "' assembly!");
			}
			return harmonyAssembly;
		}
	}
}
