using LogicAPI.Server;
using LogicLog;

namespace EcconiaCPUServerComponents.Server
{
	public class ModClass : ServerMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
		}
	}
}
