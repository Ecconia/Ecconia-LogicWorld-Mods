using System.Reflection;
using HarmonyLib;
using LogicAPI.Client;
using SUCC;

namespace DisableSuccFileReloading.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			var harmony = new Harmony("FixClientBugs");

			Logger.Warn("SUCC file reloading on file-system changes is DISABLED.");

			var RWDataFileMeth = typeof(DataFile).GetMethod("OnWatcherChanged", BindingFlags.NonPublic | BindingFlags.Instance);
			var RWHookMeth = GetType().GetMethod(nameof(rwHook), BindingFlags.Static | BindingFlags.Public);
			var RDataFileMeth = typeof(ReadOnlyDataFile).GetMethod("OnWatcherChanged", BindingFlags.NonPublic | BindingFlags.Instance);
			var RHookMeth = GetType().GetMethod(nameof(rHook), BindingFlags.Static | BindingFlags.Public);
			harmony.Patch(RWDataFileMeth, new HarmonyMethod(RWHookMeth));
			harmony.Patch(RDataFileMeth, new HarmonyMethod(RHookMeth));
		}

		public static bool rwHook(DataFile __instance)
		{
			return false;
		}

		public static bool rHook(ReadOnlyDataFile __instance)
		{
			return false;
		}
	}
}
