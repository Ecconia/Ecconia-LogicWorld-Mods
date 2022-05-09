using JimmysUnityUtilities;
using KnifeOutline;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPOutlineData
	{
		public static readonly OutlineData skippedPeg = new OutlineData(
			new Color24(150, 150, 150), //Brighter than 'invalidWire'
			OutlineLayer.Layer2);

		public static readonly OutlineData firstPeg = new OutlineData(
			new Color24(0, 150, 255),
			OutlineLayer.Layer0);

		public static readonly OutlineData firstSkippedPeg = new OutlineData(
			new Color24(80, 100, 150), //Darker/Grayer than 'firstPeg'
			OutlineLayer.Layer0);

		public static readonly OutlineData firstDiscoveredPegs = new OutlineData(
			new Color24(100, 150, 255), //Brighter than 'firstPeg'
			OutlineLayer.Layer0);

		public static readonly OutlineData middlePegs = new OutlineData(
			new Color24(0, 0, 255), //Same as 'Select'
			OutlineLayer.Layer0);

		public static readonly OutlineData secondPeg = new OutlineData(
			new Color24(255, 150, 0),
			OutlineLayer.Layer0);

		public static readonly OutlineData secondSkippedPeg = new OutlineData(
			new Color24(150, 100, 80), //Darker/Grayer than 'secondPeg'
			OutlineLayer.Layer0);

		public static readonly OutlineData secondDiscoveredPegs = new OutlineData(
			new Color24(255, 150, 100), //Brighter than 'secondPeg'
			OutlineLayer.Layer0);

		public static readonly OutlineData validWire = new OutlineData(
			new Color24(0, 255, 0), //Same as 'Valid'
			OutlineLayer.Layer1);

		public static readonly OutlineData invalidWire = new OutlineData(
			new Color24(205, 0, 0), //Same as 'Invalid'
			OutlineLayer.Layer2);

		public static readonly OutlineData validMultiWire = new OutlineData(
			new Color24(0, 100, 0), //Darker than 'validWire'
			OutlineLayer.Layer1);

		public static readonly OutlineData invalidMultiWire = new OutlineData(
			new Color24(255, 100, 100), //Brighter than 'invalidWire'
			OutlineLayer.Layer2);
	}
}
