using System.Collections.Generic;
using LogicLog;
using LogicWorld.SharedCode.Networking;

namespace EccsLogicWorldAPI.Shared.PacketWrapper
{
	//This interface is mainly for storing wrappers in a generic form.
	public interface ICustomPacketWrapper
	{
	}
	
	public class PacketHandlerWrapper<T> : PacketHandler<T>, ICustomPacketWrapper
	{
		public static readonly ILogicLogger logger = LogicLogger.For("EccsLogicWorldAPI/PacketHandlerWrapper");
		
		public readonly List<CustomPacketHandler<T>> handlers = new List<CustomPacketHandler<T>>();
		
		public PacketHandlerWrapper(PacketHandler<T> originalHandler)
		{
			handlers.Add(new CustomPacketHandler<T>.VanillaWrapper(originalHandler));
		}
		
		public void addHandlerToFront(CustomPacketHandler<T> handler)
		{
			handlers.Insert(0, handler);
		}
		
		public void addHandlerToEnd(CustomPacketHandler<T> handler)
		{
			handlers.Add(handler);
		}
		
		public override void Handle(T packet, HandlerContext context)
		{
			var isCancelled = false;
			foreach(var handler in handlers)
			{
				//TBI: Consider if custom handlers or all in general should be try-catched. Normally a failure is hard, thus don't capture them. But maybe a mod could opt-in to handling-exceptions?
				handler.handle(ref isCancelled, ref packet, context);
			}
		}
	}
}
