using JimmysUnityUtilities;
using LogicSettings;
using LogicWorld.Outlines;

namespace WireTracer.Client
{
	public static class WireTracerColors
	{
		//All clusters:
		//Linking color is the intersection between clusters, it does not make sense to have this once per cluster type.
		// The only change that could be done is to give linking separators between two non-primary clusters a different color.
		// That is currently not supported nor detected.
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.Linking")]
		private static Color24 linkingSetting { get; set; } = new Color24(200, 200, 200);
		public static OutlineData linking => new OutlineData(linkingSetting);
		
		// Previously each cluster type had its own output-peg color.
		// However, you can connect OutputPegs to multiple cluster types, making the color of the peg undefined...
		// Thus, for now there is only color. They all used to be some kind of blue anyway.
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.Output")]
		private static Color24 outputSetting { get; set; } = new Color24( 50,  50, 255);
		public static OutlineData output => new OutlineData(outputSetting);
		
		// Primary/Selected cluster:
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.Primary")]
		private static Color24 primaryNormalSetting { get; set; } = new Color24( 50, 255,  50);
		public static OutlineData primaryNormal => new OutlineData(primaryNormalSetting);
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.PrimaryInfrastructure")]
		private static Color24 primaryConnectedSetting { get; set; } = new Color24( 20, 150,  20);
		public static OutlineData primaryConnected => new OutlineData(primaryConnectedSetting);
		
		// Sourcing cluster:
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.Sourcing")]
		private static Color24 sourcingNormalSetting { get; set; } = new Color24(255,  50, 255);
		public static OutlineData sourcingNormal => new OutlineData(sourcingNormalSetting);
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.SourcingInfrastructure")]
		private static Color24 sourcingConnectedSetting { get; set; } = new Color24(150,  20, 150);
		public static OutlineData sourcingConnected => new OutlineData(sourcingConnectedSetting);
		
		// Connected cluster:
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.Connected")]
		private static Color24 connectedNormalSetting { get; set; } = new Color24(255, 255,  50);
		public static OutlineData connectedNormal => new OutlineData(connectedNormalSetting);
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.ConnectedInfrastructure")]
		private static Color24 connectedConnectedSetting { get; set; } = new Color24(150, 150,  20);
		public static OutlineData connectedConnected => new OutlineData(connectedConnectedSetting);
		
		// Draining cluster:
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.Draining")]
		private static Color24 drainingNormalSetting { get; set; } = new Color24( 50, 255, 255);
		public static OutlineData drainingNormal => new OutlineData(drainingNormalSetting);
		[Setting_ColorPicker("Ecconia.WireTracer.OutlineColor.DrainingInfrastructure")]
		private static Color24 drainingConnectedSetting { get; set; } = new Color24( 20, 150, 150);
		public static OutlineData drainingConnected => new OutlineData(drainingConnectedSetting);
	}
}
