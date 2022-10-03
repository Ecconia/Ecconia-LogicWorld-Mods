using HarmonyForLogicWorld.Shared;
using LogicAPI.Client;
using LogicWorld.SharedCode.Modding;

namespace HarmonyForLogicWorld.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			if(ModLoader.LoadedMods.ContainsKey("lwharmony"))
			{
				Logger.Warn("Could not run 'HarmonyForLogicWorld', because 'lwharmony' is installed. Pray that it installs the right version of Harmony.");
				return;
			}
			HarmonyLoader.loadHarmony(Logger, Files, true);
		}
	}
}
