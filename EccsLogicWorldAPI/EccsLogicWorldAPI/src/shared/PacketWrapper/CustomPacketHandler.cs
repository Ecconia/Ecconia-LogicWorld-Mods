using LogicWorld.SharedCode.Networking;

namespace EccsLogicWorldAPI.Shared.PacketWrapper
{
	public abstract class CustomPacketHandler<T>
	{
		public abstract void handle(ref bool isCancelled, ref T packet, HandlerContext context);
		
		public class VanillaWrapper : CustomPacketHandler<T>
		{
			private readonly PacketHandler<T> originalHandler;
			
			public VanillaWrapper(PacketHandler<T> originalHandler)
			{
				this.originalHandler = originalHandler;
			}
			
			public override void handle(ref bool isCancelled, ref T packet, HandlerContext context)
			{
				if(isCancelled)
				{
					return;
				}
				originalHandler.Handle(packet, context);
			}
		}
	}
}
