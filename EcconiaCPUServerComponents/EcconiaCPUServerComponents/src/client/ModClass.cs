using LogicAPI.Client;
using LogicLog;

namespace EcconiaCPUServerComponents.Client
{
	public class ModClass : ClientMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
		}
	}
}
