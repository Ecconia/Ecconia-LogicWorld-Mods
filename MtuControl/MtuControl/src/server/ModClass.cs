using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using Lidgren.Network;
using LogicAPI.Server;
using LogicAPI.Server.Configuration;
using LogicAPI.Server.Networking;
using LogicLog;
using LogicWorld.Server.Networking.Implementation;

namespace MtuControl.Server;

public class ModClass : ServerMod
{
	private static ILogicLogger logger;
	
	protected override void Initialize()
	{
		logger = Logger;
		// When NOT using an integrated server, limit the MTU a bit:
		var arguments = ServiceGetter.getService<LaunchOptions>();
		if (!arguments.Integrated)
		{
			setMtu(1380);
		}
	}
	
	[Command(Description = "Sets the MTU of Lidgren. Only applies to new connections.")]
	public static void setMtu(int mtu)
	{
		var networkServer = (LidgrenNetworkServer) ServiceGetter.getService<NetworkServer>();
		var lidgren = Types.checkType<NetServer>(Fields.getNonNull(Fields.getPrivate(typeof(LidgrenNetworkServer), "Lidgren"), networkServer));
		var lidgrenConfig = lidgren.Configuration;
		
		var mtuField = Fields.getPrivate(lidgrenConfig.GetType(), "m_maximumTransmissionUnit");
		var oldValue = Types.checkType<int>(mtuField.GetValue(lidgrenConfig));
		mtuField.SetValue(lidgrenConfig, mtu);
		logger.Warn($"Set MTU from {oldValue} to {mtu}");
	}
}
