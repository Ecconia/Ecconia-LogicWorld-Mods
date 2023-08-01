using EccsLogicWorldAPI.Shared.PacketWrapper;
using LogicAPI.Client;
using LogicAPI.Networking.Packets.Initialization;
using LogicLog;
using LogicWorld.SharedCode.Networking;

namespace FlexibleComponentModUsage.client
{
	public class FlexibleComponentModUsage : ClientMod
	{
		public static ILogicLogger logger;

		private RegisteredComponentManager manager;

		protected override void Initialize()
		{
			logger = Logger;
			PacketHandlerManager.getCustomPacketHandler<WorldInitializationPacket>()
				.addHandlerToFront(new Handler(this));
		}

		public void onWorldPacket(WorldInitializationPacket packet)
		{
			if(manager != null)
			{
				manager.checkForChanges();
			}
			else
			{
				manager = new RegisteredComponentManager();
			}
			manager.adjust(packet.ComponentIDsMap);
		}

		private class Handler : CustomPacketHandler<WorldInitializationPacket>
		{
			private readonly FlexibleComponentModUsage mod;

			public Handler(FlexibleComponentModUsage mod)
			{
				this.mod = mod;
			}

			public override void handle(ref bool isCancelled, ref WorldInitializationPacket packet, HandlerContext context)
			{
				if(isCancelled)
				{
					return;
				}
				mod.onWorldPacket(packet);
			}
		}
	}
}
