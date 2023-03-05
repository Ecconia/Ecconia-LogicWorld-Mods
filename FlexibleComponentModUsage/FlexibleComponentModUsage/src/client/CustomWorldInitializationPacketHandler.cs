using LogicAPI.Networking.Packets.Initialization;
using LogicWorld.SharedCode.Networking;

namespace FlexibleComponentModUsage.client
{
	public class CustomWorldInitializationPacketHandler : PacketHandler<WorldInitializationPacket>
	{
		private readonly FlexibleComponentModUsage mod;
		private readonly IPacketHandler originalHandler;

		public CustomWorldInitializationPacketHandler(FlexibleComponentModUsage mod, IPacketHandler originalHandler)
		{
			this.mod = mod;
			this.originalHandler = originalHandler;
		}

		public override void Handle(WorldInitializationPacket packet, HandlerContext context)
		{
			mod.onWorldPacket(packet);
			originalHandler.Handle(packet, context);
		}
	}
}
