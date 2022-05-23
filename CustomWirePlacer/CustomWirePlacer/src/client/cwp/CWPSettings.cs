using System;
using System.Collections.Generic;

namespace CustomWirePlacer.Client.CWP
{
	public static class CWPSettings
	{
		public static bool flipping;

		private static bool _resetFlipping = true;

		public static bool resetFlipping
		{
			get => _resetFlipping;
			set { _resetFlipping = value; }
		}

		private static bool _showDetails = true;

		public static bool showDetails
		{
			get => _showDetails;
			set { _showDetails = value; }
		}

		public static bool expandOnlyUniformDistance { get; set; }

		//TBI: Somehow option preserve last skip rate?

		//Make second group 2D, if first was.
		//Raycast in the middle of pegs.

		public static IEnumerable<CWPSetting> collectSettings()
		{
			yield return new CWPSetting
			{
				key = "CWP.Settings.ResetFlipping",
				setter = b => resetFlipping = b,
				defaultValue = resetFlipping,
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ShowDetails",
				setter = b => showDetails = b,
				defaultValue = showDetails,
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ExpandUniformDistance",
				setter = b => expandOnlyUniformDistance = b,
				defaultValue = expandOnlyUniformDistance,
				hoverKey = "CWP.Setting.ExpandUniformDistance.Description",
			};
		}
	}

	public class CWPSetting
	{
		public string key;
		public string hoverKey;
		public Action<bool> setter;
		public bool defaultValue;
		//TODO: Someone detect the text size and let that scale up the multiplier.
		// Because the language framework cannot inject this number, and languages may change it.
		// Or better, just let the text-box increase...
		public int textHeightModifier = 1;
	}
}
