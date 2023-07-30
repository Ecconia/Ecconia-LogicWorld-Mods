using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicWorld.Building;

namespace CustomWirePlacer.Client
{
	public static class Hijacker
	{
		public static void hijackWirePlacer()
		{
			var classWirePlacer = Types.findInAssembly(typeof(WireGhost), "LogicWorld.Building.WirePlacer");
			var methodEntryPoint = Methods.getPublicStatic(classWirePlacer, "PollStartWirePlacing");
			var callInsteadMethod = Methods.getPublicStatic(typeof(Hijacker), nameof(harmonyCallback));
			new Harmony("WirePlacer Hijacker").Patch(methodEntryPoint, new HarmonyMethod(callInsteadMethod));
		}

		public static bool harmonyCallback(ref bool __result)
		{
			//Oops I did it again, having a classname identical to a namespace.
			__result = CWP.CustomWirePlacer.pollStartWirePlacing();
			return false; //lets not call the original method.
		}
	}
}
