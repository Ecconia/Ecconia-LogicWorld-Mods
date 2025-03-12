using System;
using UnityEngine;

namespace EccsGuiBuilder.Client.Layouts.Helper
{
	public enum Anchor
	{
		Start,
		Center,
		End,
	}
	
	public static class SimpleAnchorExtensions
	{
		public static TextAnchor toTextAnchor(this Anchor anchor)
		{
			// More modern switch statements are not supported by the LW-Compiler.
			switch (anchor)
			{
				case Anchor.Start:
					return TextAnchor.UpperLeft;
				case Anchor.Center:
					return TextAnchor.MiddleCenter;
				case Anchor.End:
					return TextAnchor.LowerRight;
				default:
					throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
			}
		}
	}
}
