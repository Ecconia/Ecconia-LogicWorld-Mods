using LogicAPI.Server;
using LogicLog;
using ServerOnlyMods.Server;

namespace ServerLoadAccelerator.server
{
	public class ModClass : ServerMod, IServerSideOnlyMod
	{
		public static ILogicLogger logger;

		protected override void Initialize()
		{
			logger = Logger;
			try
			{
				InitWireClustersLater.init();
			}
			catch(RefHelEx e)
			{
				logger.Error("Reflection error, cannot optimize loading times: " + e.Message);
			}
		}
	}
}
