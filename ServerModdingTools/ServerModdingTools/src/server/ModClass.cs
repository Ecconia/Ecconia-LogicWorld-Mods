using LogicAPI.Server;
using ServerOnlyMods.Server;

namespace ServerModdingTools.Server
{
	public class ModClass : ServerMod, IServerSideOnlyMod
	{
		protected override void Initialize()
		{
			PlayerJoiningHook.inject(Logger);
		}
	}
}
