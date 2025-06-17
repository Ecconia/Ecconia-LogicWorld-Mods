using LogicAPI.Client;
using LogicWorld.SharedCode.Modding;

namespace HarmonyForLogicWorld.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			if(ModRegistry.IsModInstalled("lwharmony"))
			{
				Logger.Warn("The mod 'lwharmony' is installed alongside with 'HarmonyForLogicWorld'. Which means that it is undefined which version of Harmony is installed and being used. Pray that things run properly. Recommendation: Remove mod 'lwharmony'");
			}
		}
	}
}
