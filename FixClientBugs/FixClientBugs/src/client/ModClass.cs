using FixClientBugs.Client.Fixes;
using HarmonyLib;
using LogicAPI.Client;

namespace FixClientBugs.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			Harmony harmony = new Harmony("FixClientBugs");
			FixUndoNRE.init(Logger, harmony);
			FixGhostUpdating.init(Logger, harmony);
		}
	}
}
