using HarmonyForLogicWorld.Shared;
using LogicAPI.Server;

namespace HarmonyForLogicWorld.Server
{
	public class ModClass : ServerMod
	{
		protected override void Initialize()
		{
			HarmonyLoader.loadHarmony(Logger, Files, false);
		}
	}
}
