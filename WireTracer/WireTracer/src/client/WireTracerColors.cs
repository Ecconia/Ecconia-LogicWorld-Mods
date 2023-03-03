using JimmysUnityUtilities;
using LogicWorld.Outlines;

namespace WireTracer.Client
{
	public static class WireTracerColors
	{
		//All clusters:
		//Linking color is the intersection between clusters, it does not make sense to have this once per cluster type.
		// The only change that could be done is to give linking separators between two non-primary clusters a different color.
		// That is currently not supported nor detected.
		public static readonly OutlineData linking = new OutlineData(new Color24(200, 200, 200));
		
		//Primary cluster:
		public static readonly OutlineData primaryNormal    = new OutlineData(new Color24( 50, 255,  50));
		public static readonly OutlineData primaryConnected = new OutlineData(new Color24( 20, 150,  20));
		public static readonly OutlineData primaryOutput    = new OutlineData(new Color24( 50,  50, 255));
		
		//Sourcing cluster:
		public static readonly OutlineData sourcingNormal    = new OutlineData(new Color24(255,  50, 255));
		public static readonly OutlineData sourcingConnected = new OutlineData(new Color24(150,  20, 150));
		public static readonly OutlineData sourcingOutput    = new OutlineData(new Color24( 80,   0, 255));
		
		//Connected cluster:
		public static readonly OutlineData connectedNormal    = new OutlineData(new Color24(255, 255,  50));
		public static readonly OutlineData connectedConnected = new OutlineData(new Color24(150, 150,  20));
		public static readonly OutlineData connectedOutput    = new OutlineData(new Color24( 80,  80, 255));
		
		//Draining cluster:
		public static readonly OutlineData drainingNormal    = new OutlineData(new Color24( 50, 255, 255));
		public static readonly OutlineData drainingConnected = new OutlineData(new Color24( 20, 150, 150));
		public static readonly OutlineData drainingOutput    = new OutlineData(new Color24(  0,  50, 150));
	}
}
