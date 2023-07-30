using System;
using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	public static class Types
	{
		//Random helper:
		
		public static T checkType<T>(object value, string message = null)
		{
			if(value is T cast)
			{
				return cast;
			}
			if(message == null)
			{
				message = "Expected type " + typeof(T).Name + ", but got type " + value.GetType().Name;
			}
			throw new AccessHelperException(message);
		}
		
		//Lookups:
		
		public static Type getType(Assembly assembly, string name)
		{
			Type type = assembly.GetType(name);
			if(type == null)
			{
				throw new AccessHelperException("Could not find class '" + name + "' in assembly '" + assembly.FullName + "'.");
			}
			return type;
		}
		
		public static Type findInAssembly(Type neighborType, string targetClassName)
		{
			Type type = neighborType.Assembly.GetType(targetClassName);
			if(type == null)
			{
				throw new AccessHelperException("Could not find class '" + targetClassName + "' in same assembly as '" + neighborType + "'.");
			}
			return type;
		}
		
		//Instance creator:
		
		public static object createInstance(Type type, params object[] args)
		{
			object instance = Activator.CreateInstance(type, args);
			if(instance == null)
			{
				throw new AccessHelperException("Was not able to create type '" + type + "' with arguments '" + args + "'.");
			}
			return instance;
		}
	}
}
