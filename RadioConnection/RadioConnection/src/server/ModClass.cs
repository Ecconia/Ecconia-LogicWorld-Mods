using EccsLogicWorldAPI.Server;
using LogicAPI.Server;

namespace RadioConnection.Server
{
	public class ModClass : ServerMod
	{
		protected override void Initialize()
		{
			VirtualInputPegPool.ensureInitialized();
		}
	}
}
