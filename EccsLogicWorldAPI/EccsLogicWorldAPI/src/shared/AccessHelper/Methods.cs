using System;
using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	public static class Methods
	{
		//Accessor: any
		
		public static MethodInfo get(Type type, string name)
		{
			return get(type, name, Bindings.any, "");
		}
		
		public static MethodInfo get(object obj, string name)
		{
			return get(obj.GetType(), name);
		}
		
		//Accessor: private
		
		public static MethodInfo getPrivate(Type type, string name)
		{
			return get(type, name, Bindings.privateInst, "private");
		}
		
		public static MethodInfo getPrivate(object obj, string name)
		{
			return getPrivate(obj.GetType(), name);
		}
		
		//Accessor: private static
		
		public static MethodInfo getPrivateStatic(Type type, string name)
		{
			return get(type, name, Bindings.privateStatic, "private static");
		}
		
		public static MethodInfo getPrivateStatic(object obj, string name)
		{
			return getPrivateStatic(obj.GetType(), name);
		}
		
		//Accessor: public
		
		public static MethodInfo getPublic(Type type, string name)
		{
			return get(type, name, Bindings.publicInst, "public");
		}
		
		public static MethodInfo getPublic(object obj, string name)
		{
			return getPublic(obj.GetType(), name);
		}
		
		//Accessor: public static
		
		public static MethodInfo getPublicStatic(Type type, string name)
		{
			return get(type, name, Bindings.publicStatic, "public static");
		}
		
		public static MethodInfo getPublicStatic(object obj, string name)
		{
			return getPublicStatic(obj.GetType(), name);
		}
		
		//Internal getter
		
		private static MethodInfo get(Type type, string name, BindingFlags flags, string accessors)
		{
			MethodInfo method = type.GetMethod(name, flags);
			if(method == null)
			{
				if(accessors.Length != 0)
				{
					accessors += ' ';
				}
				throw new AccessHelperException("Could not find " + accessors + " method '" + name + "' in " + type.Name);
			}
			return method;
		}
	}
}
