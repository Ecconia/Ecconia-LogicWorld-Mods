using LogicAPI.Client;

namespace RandomDebugCollection.Client
{
	public class RandomDebugCollection : ClientMod
	{
		protected override void Initialize()
		{
			StartupArgumentsHook.init();
		}
	}
}
