using EcconiasChaosClientMod.Client.Lighting;
using LogicAPI.Client;
using LogicLog;

namespace EcconiasChaosClientMod.Client
{
	public class ModClass : ClientMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger; //Required for some static code initialization.
			Skybox.init(Files, Logger);
		}
	}
}
