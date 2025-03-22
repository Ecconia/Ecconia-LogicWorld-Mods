using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicWorld.Building;
using WireTracer.Client.Tool;

namespace WireTracer.Client
{
	//TODO: Preferably add this to my API mod, so that others can use this. Also make it more generic. Well ideally LW fixes its FPI, to allow Mod callbacks...
	public class WireTracerHook
	{
		public static void init()
		{
			var target = Methods.getPrivateStatic(typeof(StuffRotater), "RunFirstPersonWireRotations");
			var hook = Methods.getPublicStatic(typeof(WireTracerHook), nameof(postfixHook));
			var harmony = new Harmony("WireTracer");
			harmony.Patch(target, postfix: new HarmonyMethod(hook));
		}
		
		public static void postfixHook(ref bool rotatedWire)
		{
			if(rotatedWire)
			{
				//If the result is true, the wire-rotation action was executed and first person interaction is done.
				return;
			}
			rotatedWire = WireTracerTool.RunFirstPersonClusterHighlighting();
		}
	}
}
