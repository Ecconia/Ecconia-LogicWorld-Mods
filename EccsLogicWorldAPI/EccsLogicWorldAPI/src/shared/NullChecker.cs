using System;

namespace EccsLogicWorldAPI.Shared
{
	public static class NullChecker
	{
		public static void check(object value, string message)
		{
			if(value == null)
			{
				throw new Exception(message);
			}
		}
	}
}
