//Needed for 'Exception':
using System;
//Needed for Harmony:
using HarmonyLib;
//Needed for 'ILogicLogger':
using LogicLog;
//Needed for 'SceneAndNetworkManager':
using LogicWorld;

namespace RandomDebugCollection.Client
{
	public static class StacktraceToLog
	{
		public static void Initialize(ILogicLogger logger)
		{
			Patch.logger = logger;
			new Harmony("RandomDebugCollection.StacktraceToLog").PatchAll();
		}
	}

	[HarmonyPatch(typeof(SceneAndNetworkManager), nameof(SceneAndNetworkManager.TriggerErrorScreen), new Type[] {typeof(Exception)})]
	public static class Patch
	{
		public static ILogicLogger logger;

		public static void Prefix(Exception exception)
		{
			logger.Error("Captured exception:\n" + exception);
		}
	}
}
