using System;
using System.Reflection;
using HarmonyLib;
using LogicWorld.Building;

namespace CustomWirePlacer.Client
{
	public static class Hijacker
	{
		public static void hijackWirePlacer()
		{
			var classWirePlacer = typeof(WireGhost).Assembly.GetType("LogicWorld.Building.WirePlacer");
			if(classWirePlacer == null)
			{
				throw new Exception("Could not find WirePlacer class. CWE won't work.");
			}
			var methodEntryPoint = classWirePlacer.GetMethod("PollStartWirePlacing", BindingFlags.Public | BindingFlags.Static);
			if(methodEntryPoint == null)
			{
				throw new Exception("Could not find method 'WirePlacer.PollStartWirePlacing()'. CWE won't work.");
			}
			var callInsteadMethod = typeof(Hijacker).GetMethod(nameof(Hijacker.harmonyCallback), BindingFlags.Public | BindingFlags.Static);
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
