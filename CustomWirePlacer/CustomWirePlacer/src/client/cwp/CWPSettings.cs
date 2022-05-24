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

		private static bool _showDetails = false;

		public static bool showDetails
		{
			get => _showDetails;
			set { _showDetails = value; }
		}
		
		public static bool raycastAtBottomOfPegs { get; set; }

		public static bool expandOnlyUniformDistance { get; set; }

		public static bool skipScrollInBinarySteps { get; set; }

		private static bool _resetSkipOffsetWhenNotSkipping = true;

		public static bool resetSkipOffsetWhenNotSkipping
		{
			get => _resetSkipOffsetWhenNotSkipping;
			set
			{
				_resetSkipOffsetWhenNotSkipping = value;
				if(value && CustomWirePlacer.isActive())
				{
					CustomWirePlacer.getCurrentGroup().getCurrentAxis().checkSkipOffsetReset();
				}
			}
		}

		//TBI: Somehow option preserve last skip rate?

		//Make second group 2D, if first was.

		public static IEnumerable<CWPSetting> collectSettings()
		{
			// yield return new CWPSetting
			// {
			// 	key = "CWP.Settings.ResetFlipping",
			// 	setter = b => resetFlipping = b,
			// 	defaultValue = resetFlipping,
			// };
			// yield return new CWPSetting
			// {
			// 	key = "CWP.Setting.ShowDetails",
			// 	setter = b => showDetails = b,
			// 	defaultValue = showDetails,
			// };
			yield return new CWPSetting
			{
				key = "CWP.Setting.ExpandUniformDistance",
				setter = b => expandOnlyUniformDistance = b,
				defaultValue = expandOnlyUniformDistance,
				hoverKey = "CWP.Setting.ExpandUniformDistance.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.SkipScrollInBinarySteps",
				setter = b => skipScrollInBinarySteps = b,
				defaultValue = skipScrollInBinarySteps,
				hoverKey = "CWP.Setting.SkipScrollInBinarySteps.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ResetSkipOffsetWhenNotSkipping",
				setter = b => resetSkipOffsetWhenNotSkipping = b,
				defaultValue = resetSkipOffsetWhenNotSkipping,
				hoverKey = "CWP.Setting.ResetSkipOffsetWhenNotSkipping.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.RaycastAtBottomOfPegs",
				setter = b => raycastAtBottomOfPegs = b,
				defaultValue = raycastAtBottomOfPegs,
				hoverKey = "CWP.Setting.RaycastAtBottomOfPegs.Description",
			};
		}
	}

	public class CWPSetting
	{
		public string key;
		public string hoverKey;
		public Action<bool> setter;
		public bool defaultValue;
	}
}
