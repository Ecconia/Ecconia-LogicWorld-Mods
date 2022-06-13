using System;
using System.Collections.Generic;
using CustomWirePlacer.Client.Windows;

namespace CustomWirePlacer.Client.CWP
{
	public static class CWPSettings
	{
		private static bool _showStatusOverlay = true;

		public static bool showStatusOverlay
		{
			get => _showStatusOverlay;
			private set
			{
				_showStatusOverlay = value;
				if(CustomWirePlacer.isActive())
				{
					CWPStatusOverlay.setVisible(value);
				}
			}
		}
		
		private static bool _showStatusOverlayModifiers = true;

		public static bool showStatusOverlayModifiers
		{
			get => _showStatusOverlayModifiers;
			private set
			{
				_showStatusOverlayModifiers = value;
				CWPStatusOverlay.setDirtySettingsConfig();
			}
		}
		
		private static bool _showStatusOverlaySettings;

		public static bool showStatusOverlaySettings
		{
			get => _showStatusOverlaySettings;
			private set
			{
				_showStatusOverlaySettings = value;
				CWPStatusOverlay.setDirtySettingsConfig();
			}
		}

		public static bool allowStartingWithOnePegGroup { get; private set; }

		private static bool _raycastAtBottomOfPegs;

		public static bool raycastAtBottomOfPegs
		{
			get => _raycastAtBottomOfPegs;
			private set
			{
				_raycastAtBottomOfPegs = value;
				CustomWirePlacer.raycastLine.refresh();
				CWPStatusOverlay.setDirtySettings();
			}
		}

		private static bool _showRaycastRay;

		public static bool showRaycastRay
		{
			get => _showRaycastRay;
			private set
			{
				_showRaycastRay = value;
				CustomWirePlacer.raycastLine.refresh();
			}
		}

		private static bool _expandOnlyUniformDistance;

		public static bool expandOnlyUniformDistance
		{
			get => _expandOnlyUniformDistance;
			private set
			{
				_expandOnlyUniformDistance = value;
				CWPStatusOverlay.setDirtySettings();
			}
		}

		private static bool _resetSkipOffsetWhenNotSkipping = true;

		public static bool resetSkipOffsetWhenNotSkipping
		{
			get => _resetSkipOffsetWhenNotSkipping;
			private set
			{
				_resetSkipOffsetWhenNotSkipping = value;
				if(value && CustomWirePlacer.isActive())
				{
					CustomWirePlacer.getCurrentGroup().getCurrentAxis().checkSkipOffsetReset();
				}
			}
		}

		public static bool scrollSkipInMulDivOfTwoSteps { get; private set; }

		private static bool _roundSkipOffsetToNextBinaryNumber = true;

		public static bool roundSkipOffsetToNextBinaryNumber
		{
			get => _roundSkipOffsetToNextBinaryNumber;
			private set
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
			private set
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
				key = "CWP.Setting.ShowStatusOverlay",
				setter = b => showStatusOverlay = b,
				defaultValue = showStatusOverlay,
				hoverKey = "CWP.Setting.ShowStatusOverlay.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ShowStatusOverlayModifiers",
				setter = b => showStatusOverlayModifiers = b,
				defaultValue = showStatusOverlayModifiers,
				hoverKey = "CWP.Setting.ShowStatusOverlayModifiers.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ShowStatusOverlaySettings",
				setter = b => showStatusOverlaySettings = b,
				defaultValue = showStatusOverlaySettings,
				hoverKey = "CWP.Setting.ShowStatusOverlaySettings.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.RaycastAtBottomOfPegs",
				setter = b => raycastAtBottomOfPegs = b,
				defaultValue = raycastAtBottomOfPegs,
				hoverKey = "CWP.Setting.RaycastAtBottomOfPegs.Description",
			};
			yield return new CWPSetting
			{
				key = "CWP.Setting.ShowRaycastRay",
				setter = b => showRaycastRay = b,
				defaultValue = showRaycastRay,
				hoverKey = "CWP.Setting.ShowRaycastRay.Description",
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
			yield return new CWPSetting
			{
				key = "CWP.Setting.AllowStartingWithOnePegGroup",
				setter = b => allowStartingWithOnePegGroup = b,
				defaultValue = allowStartingWithOnePegGroup,
				hoverKey = "CWP.Setting.AllowStartingWithOnePegGroup.Description",
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
