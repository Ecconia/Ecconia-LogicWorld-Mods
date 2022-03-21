using LogicAPI.Server;

namespace ServerOnlyMods.Server
{
	public class ModClass : ServerMod, IServerSideOnlyMod
	{
		protected override void Initialize()
		{
			BetterClientModVerifier.inject(Logger);
		}
	}
}
