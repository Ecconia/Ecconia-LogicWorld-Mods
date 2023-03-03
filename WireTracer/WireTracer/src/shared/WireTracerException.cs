using System;

namespace WireTracer.Shared
{
	public class WireTracerException : Exception
	{
		public WireTracerException(string message) : base(message)
		{
		}
	}
}
