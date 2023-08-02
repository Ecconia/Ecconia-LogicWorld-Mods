using CustomChatManager.Server.ChatServices;
using CustomChatManager.Server.Commands;
using EccsLogicWorldAPI.Shared.PacketWrapper;
using LogicAPI.Networking.Packets.Client;
using LogicAPI.Server;

namespace CustomChatManager.Server
{
	public class ModClass : ServerMod
	{
		protected override void Initialize()
		{
			CustomChatManager chatManager = new CustomChatManager();

			//Setup the chat manager:
			{
				//Comment out a feature if you do not like it:
				chatManager.addProcessor(new VerifyMessageLength()); //This is the only validation done by the game.
				chatManager.addProcessor(new VerifySenderName()); //Stops a player from fooling others, by fixing its message.
				chatManager.addProcessor(new CommandManager(Logger));
				{
					CommandManager.instance.register(new CommandTPS());
					CommandManager.instance.register(new CommandList());
					CommandManager.instance.register(new CommandSaveSize());
					CommandManager.instance.register(new CommandMods());
				}
			}

			PacketHandlerManager.getCustomPacketHandler<ChatMessageSentPacket>()
				.addHandlerToFront(new CustomChatManager.CustomChatPacketHandler(chatManager));
		}
	}
}
