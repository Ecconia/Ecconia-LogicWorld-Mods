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
			
			// CircuitColor.setCircuitColor(new Color24(255, 140, 10)); // Orange circuits
			// CircuitColor.setCircuitColor(new Color24(10, 150, 10)); // Green circuits (Clashes with wire outlines)
		}
	}
}
