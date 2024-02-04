using System;
using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	public static class Properties
	{
		//Accessor: any
		
		public static PropertyInfo get(Type type, string name)
		{
			return get(type, name, Bindings.any, "");
		}
		
		public static PropertyInfo get(object obj, string name)
		{
			return get(obj.GetType(), name);
		}
		
		//Accessor: private
		
		public static PropertyInfo getPrivate(Type type, string name)
		{
			return get(type, name, Bindings.privateInst, "private");
		}
		
		public static PropertyInfo getPrivate(object obj, string name)
		{
			return getPrivate(obj.GetType(), name);
		}
		
		//Accessor: private static
		
		public static PropertyInfo getPrivateStatic(Type type, string name)
		{
			return get(type, name, Bindings.privateStatic, "private static");
		}
		
		public static PropertyInfo getPrivateStatic(object obj, string name)
		{
			return getPrivateStatic(obj.GetType(), name);
		}
		
		//Accessor: public
		
		public static PropertyInfo getPublic(Type type, string name)
		{
			return get(type, name, Bindings.publicInst, "public");
		}
		
		public static PropertyInfo getPublic(object obj, string name)
		{
			return getPublic(obj.GetType(), name);
		}
		
		//Accessor: public static
		
		public static PropertyInfo getPublicStatic(Type type, string name)
		{
			return get(type, name, Bindings.publicStatic, "public static");
		}
		
		public static PropertyInfo getPublicStatic(object obj, string name)
		{
			return getPublicStatic(obj.GetType(), name);
		}
		
		//Internal getter
		
		private static PropertyInfo get(Type type, string name, BindingFlags flags, string accessors)
		{
			var field = type.GetProperty(name, flags);
			if(field == null)
			{
				if(accessors.Length != 0)
				{
					accessors += ' ';
				}
				throw new AccessHelperException("Could not find " + accessors + " property '" + name + "' in " + type.Name);
			}
			return field;
		}
	}
}
