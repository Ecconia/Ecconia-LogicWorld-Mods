using System;
using System.Reflection;

namespace ServerLoadAccelerator.server
{
	public static class ReflectionHelper
	{
		public const BindingFlags anyBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		public static MethodInfo getMethod(Type type, string name)
		{
			MethodInfo method = type.GetMethod(name, anyBinding);
			if(method == null)
			{
				throw new RefHelEx("Could not find method '" + name + "' in " + type.Name);
			}
			return method;
		}

		public static MethodInfo getMethod(Type type, string className, string methodName)
		{
			Type newType = type.Assembly.GetType(className);
			if(newType == null)
			{
				throw new RefHelEx("Could not find class '" + className + "'");
			}
			return getMethod(newType, methodName);
		}
	}

	public class RefHelEx : Exception
	{
		public RefHelEx(string message) : base(message)
		{
		}
	}
}
