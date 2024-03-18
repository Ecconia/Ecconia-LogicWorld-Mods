using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicAPI;
using LogicLog;

namespace RandomDebugCollection.Shared
{
	public static class VersionOverride
	{
		private static Version overwriteVersion;
		
		public static void overrideVersion(string version)
		{
			if(!Version.TryParse(version, out overwriteVersion))
			{
				var logger = LogicLogger.For("RandomDebugCollection-VersionOverride");
				logger.Info($"Could not parse provided version! Provided: '{version}'");
				Environment.Exit(1);
				return;
			}
			
			// Hijack game class and patch the version getter:
			var property = Properties.getPublicStatic(typeof(Game), nameof(Game.Version)).GetMethod;
			var patch = Methods.getPrivateStatic(typeof(VersionOverride), nameof(versionPatch));
			new Harmony("Version patch").Patch(property, null, new HarmonyMethod(patch));
		}
		
		private static void versionPatch(out Version __result)
		{
			__result = overwriteVersion;
		}
	}
}
