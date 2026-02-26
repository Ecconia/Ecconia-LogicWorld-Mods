using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;

namespace EccsLogicWorldAPI.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
		}
		
		protected override void GameStartupCompleted()
		{
			// Currently there is no good way to delay the initialization of WorldHooks to a point after mod loading, besides always initializing it.
			// Eventually the game will have these callbacks itself. The overhead of always initializing WorldHook is negligible.
			WorldHook.init();
		}
	}
}
