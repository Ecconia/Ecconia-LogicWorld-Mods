using JimmysUnityUtilities;
using KnifeOutline;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public static class CWPOutlineData
	{
		// ##### Current axis: #####

		public static readonly OutlineData skippedPeg = new OutlineData(
			new Color24(150, 150, 150));

		public static readonly OutlineData firstDiscoveredPegs = new OutlineData(
			new Color24(89, 230, 255));

		public static readonly OutlineData firstPeg = new OutlineData(
			new Color24(0, 167, 255));

		public static readonly OutlineData firstSkippedPeg = new OutlineData(
			new Color24(80, 100, 150));

		public static readonly OutlineData middlePegs = new OutlineData(
			new Color24(96, 96, 255));

		public static readonly OutlineData secondPeg = new OutlineData(
			new Color24(178, 0, 255));

		public static readonly OutlineData secondDiscoveredPegs = new OutlineData(
			new Color24(255, 127, 255));

		// First group backed axis:

		public static readonly OutlineData backedGroupOneFirst = new OutlineData(
			new Color24(212, 188, 0));

		public static readonly OutlineData backedGroupOne = new OutlineData(
			new Color24(192, 115, 0));

		// White/Blacklist:

		public static readonly OutlineData whitelistedPeg = new OutlineData(
			new Color24(0, 255, 0));

		public static readonly OutlineData blacklistedPeg = new OutlineData(
			new Color24(255, 0, 0));

		// ##### Wires: #####

		public static readonly OutlineData validWire = new OutlineData(
			new Color24(0, 255, 0), //Same as 'Valid'
			OutlineLayer.Layer1);

		public static readonly OutlineData invalidWire = new OutlineData(
			new Color24(205, 0, 0), //Same as 'Invalid'
			OutlineLayer.Layer2);

		// 1-Group Multi-Wire:

		public static readonly OutlineData validMultiWire = new OutlineData(
			new Color24(0, 100, 0), //Darker than 'validWire'
			OutlineLayer.Layer1);

		public static readonly OutlineData invalidMultiWire = new OutlineData(
			new Color24(255, 100, 100), //Brighter than 'invalidWire'
			OutlineLayer.Layer2);
	}
}
