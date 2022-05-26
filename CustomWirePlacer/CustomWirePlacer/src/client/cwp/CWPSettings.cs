using System;
using System.Collections.Generic;

namespace CustomWirePlacer.Client.CWP
{
	public static class CWPSettings
	{
		public static bool flipping; //TODO: Should this be here for now?

		private static bool _showDetails; //TODO: Make window for this.

		public static bool showDetails
		{
			get => _showDetails;
			set { _showDetails = value; }
		}

		public static bool raycastAtBottomOfPegs { get; set; }

		public static bool expandOnlyUniformDistance { get; set; }

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
		
		public static bool scrollSkipInMulDivOfTwoSteps { get; set; }

		private static bool _roundSkipOffsetToNextBinaryNumber = true;

		public static bool roundSkipOffsetToNextBinaryNumber
		{
			get => _roundSkipOffsetToNextBinaryNumber;
			set
			{
				_roundSkipOffsetToNextBinaryNumber = value;
				if(value && CustomWirePlacer.isActive())
				{
					CustomWirePlacer.getCurrentGroup().getCurrentAxis().roundSkipOffsetToBinary(true);
					CustomWirePlacer.updateWireGhosts();
				}
			}
		}

		private static bool _connectPegsInOneGroupWithEachOther = true;

		public static bool connectPegsInOneGroupWithEachOther
		{
			get => _connectPegsInOneGroupWithEachOther;
			set
			{
				_connectPegsInOneGroupWithEachOther = value;
				if(CustomWirePlacer.isActive())
				{
					CustomWirePlacer.updateWireGhosts();
				}
			}
		}

		//TBI: Somehow option preserve last skip rate?

		//Make second group 2D, if first was.

		public static IEnumerable<CWPSetting> collectSettings()
		{
			yield return new CWPSetting
			{
				key = "CWP.Setting.RaycastAtBottomOfPegs",
				setter = b => raycastAtBottomOfPegs = b,
				defaultValue = raycastAtBottomOfPegs,
				hoverKey = "CWP.Setting.RaycastAtBottomOfPegs.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ExpandUniformDistance",
				setter = b => expandOnlyUniformDistance = b,
				defaultValue = expandOnlyUniformDistance,
				hoverKey = "CWP.Setting.ExpandUniformDistance.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ConnectPegsInOneGroupWithEachOther",
				setter = b => connectPegsInOneGroupWithEachOther = b,
				defaultValue = connectPegsInOneGroupWithEachOther,
				hoverKey = "CWP.Setting.ConnectPegsInOneGroupWithEachOther.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ScrollSkipInMulDivOfTwoSteps",
				setter = b => scrollSkipInMulDivOfTwoSteps = b,
				defaultValue = scrollSkipInMulDivOfTwoSteps,
				hoverKey = "CWP.Setting.ScrollSkipInMulDivOfTwoSteps.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.RoundSkipOffsetToNextBinaryNumber",
				setter = b => roundSkipOffsetToNextBinaryNumber = b,
				defaultValue = roundSkipOffsetToNextBinaryNumber,
				hoverKey = "CWP.Setting.RoundSkipOffsetToNextBinaryNumber.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ResetSkipOffsetWhenNotSkipping",
				setter = b => resetSkipOffsetWhenNotSkipping = b,
				defaultValue = resetSkipOffsetWhenNotSkipping,
				hoverKey = "CWP.Setting.ResetSkipOffsetWhenNotSkipping.Description",
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
