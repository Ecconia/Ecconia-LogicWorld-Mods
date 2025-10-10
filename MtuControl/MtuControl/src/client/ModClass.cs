using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicAPI.Client;
using LogicLog;
using LogicWorld.Networking;
using LogicWorld.Networking.Implementation;

namespace MtuControl.Client;

public class ModClass : ClientMod
{
	private static ILogicLogger logger;
	
	protected override void Initialize()
	{
		logger = Logger;
		// Comment this in, to change MTU by default:
		// setMtu(1380);
	}
	
	[Command(Description = "Sets the MTU of Lidgren. Only applies to new connections.")]
	public static void setMtu(int mtu)
	{
		var networkClient = Types.checkType<LidgrenNetworkClient>(Fields.getNonNull(Fields.getPrivateStatic(typeof(GameNetwork), "Client")));
		var lidgren = networkClient.Lidgren;
		var lidgrenConfig = lidgren.Configuration;
		
		var mtuField = Fields.getPrivate(lidgrenConfig.GetType(), "m_maximumTransmissionUnit");
		var oldValue = Types.checkType<int>(mtuField.GetValue(lidgrenConfig));
		mtuField.SetValue(lidgrenConfig, mtu);
		logger.Warn($"Set MTU from {oldValue} to {mtu}");
	}
}
