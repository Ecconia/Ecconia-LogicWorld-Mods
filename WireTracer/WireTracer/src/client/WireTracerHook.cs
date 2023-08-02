using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicWorld.Building;
using WireTracer.Client.Tool;

namespace WireTracer.Client
{
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
