using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;

namespace RandomDebugCollection.Client
{
	public static class DisablePlayerMovement
	{
		public static void disablePlayerMovement()
		{
			var a = Methods.getPublic(typeof(LogicWorld.Networking.SenderShortcuts), "PlayerPosition");
			var patch = Methods.getPrivateStatic(typeof(DisablePlayerMovement), nameof(doNothing));
			new Harmony("RandomDebugCollection").Patch(a, new HarmonyMethod(patch));
		}
		
		private static bool doNothing()
		{
			return false;
		}
	}
}
