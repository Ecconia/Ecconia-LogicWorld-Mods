using EcconiasChaosClientMod.Client.Lighting;
using LogicAPI.Client;

namespace EcconiasChaosClientMod.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			Skybox.init(Files, Logger);
		}
	}
}
