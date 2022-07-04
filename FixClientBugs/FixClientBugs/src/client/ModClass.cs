using HarmonyLib;
using LogicAPI.Client;
using LogicLog;

namespace FixClientBugs.Client
{
	public class ModClass : ClientMod
	{
		public static ILogicLogger logger;

		protected override void Initialize()
		{
			logger = Logger;
			var harmony = new Harmony("FixClientBugs");
			FixBrokenHistory.init(harmony);
			FixFlyingWithNoclip.init(harmony);
		}
	}
}
