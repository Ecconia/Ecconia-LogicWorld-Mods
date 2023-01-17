using LogicAPI.Client;
using LogicLog;

namespace EccsWindowHelper.Client
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
