using System;
using System.Reflection;
using HarmonyLib;
using LogicWorld.Building;
using WireTracer.Client.Tool;

namespace WireTracer.Client
{
	public class WireTracerHook
	{
		public static void init()
		{
			MethodInfo target = typeof(StuffRotater).GetMethod("RunFirstPersonWireRotations", BindingFlags.NonPublic | BindingFlags.Static);
			if(target == null)
			{
				throw new Exception("Did not find 'RunFirstPersonWireRotations' in 'StuffRotater'. Cannot hook WireTracer.");
			}
			MethodInfo hook = typeof(WireTracerHook).GetMethod(nameof(WireTracerHook.postfixHook), BindingFlags.Public | BindingFlags.Static);
			Harmony harmony = new Harmony("WireTracer");
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
