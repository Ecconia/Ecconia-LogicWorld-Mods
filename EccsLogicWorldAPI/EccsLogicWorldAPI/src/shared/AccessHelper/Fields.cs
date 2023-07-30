using System;
using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	public static class Fields
	{
		//Random helpers:
		
		public static object getNonNull(FieldInfo field, string message = "Value of field is unexpectedly null.")
		{
			var value = field.GetValue(null);
			NullChecker.check(value, message);
			return value;
		}
		
		public static object getNonNull(FieldInfo field, object instance, string message = "Value of field is unexpectedly null.")
		{
			var value = field.GetValue(instance);
			NullChecker.check(value, message);
			return value;
		}
		
		//Accessor: any
		
		public static FieldInfo get(Type type, string name)
		{
			return get(type, name, Bindings.any, "");
		}
		
		public static FieldInfo get(object obj, string name)
		{
			return get(obj.GetType(), name);
		}
		
		//Accessor: private
		
		public static FieldInfo getPrivate(Type type, string name)
		{
			return get(type, name, Bindings.privateInst, "private");
		}
		
		public static FieldInfo getPrivate(object obj, string name)
		{
			return getPrivate(obj.GetType(), name);
		}
		
		//Accessor: private static
		
		public static FieldInfo getPrivateStatic(Type type, string name)
		{
			return get(type, name, Bindings.privateStatic, "private static");
		}
		
		public static FieldInfo getPrivateStatic(object obj, string name)
		{
			return getPrivateStatic(obj.GetType(), name);
		}
		
		//Accessor: public
		
		public static FieldInfo getPublic(Type type, string name)
		{
			return get(type, name, Bindings.publicInst, "public");
		}
		
		public static FieldInfo getPublic(object obj, string name)
		{
			return getPublic(obj.GetType(), name);
		}
		
		//Accessor: public static
		
		public static FieldInfo getPublicStatic(Type type, string name)
		{
			return get(type, name, Bindings.publicStatic, "public static");
		}
		
		public static FieldInfo getPublicStatic(object obj, string name)
		{
			return getPublicStatic(obj.GetType(), name);
		}
		
		//Internal getter
		
		private static FieldInfo get(Type type, string name, BindingFlags flags, string accessors)
		{
			FieldInfo field = type.GetField(name, flags);
			if(field == null)
			{
				if(accessors.Length != 0)
				{
					accessors += ' ';
				}
				throw new AccessHelperException("Could not find " + accessors + " field '" + name + "' in " + type.Name);
			}
			return field;
		}
	}
}
