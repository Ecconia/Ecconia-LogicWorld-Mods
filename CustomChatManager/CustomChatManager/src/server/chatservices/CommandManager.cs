using System;
using System.Collections.Generic;
using System.Text;
using CustomChatManager.Server.Commands;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Networking.Packets.Server;
using LogicAPI.Server.Networking;
using LogicWorld.Server;
using ServerModdingTools.Server;

namespace CustomChatManager.Server.ChatServices
{
	public static class SendExtension
	{
		public static void sendMessage(this NetworkServer server, Connection connection, String content)
		{
			server.sendMessage(connection, content, Color24.White);
		}

		public static void sendMessage(this NetworkServer server, Connection connection, String content, Color24 color)
		{
			server.Send(connection, new ChatMessageBroadcastPacket()
			{
				Data = new ChatMessageData
				{
					Color = color,
					MessageContent = content,
					Sender = null,
				},
			});
		}
	}

	public class CommandManager : IChatService
	{
		public static CommandManager instance; //Usable for other mods.
		private static NetworkServer server;

		private Dictionary<String, ICommand> commands = new Dictionary<string, ICommand>();

		public void register(ICommand command)
		{
			commands.Add(command.name.ToLower(), command);
		}
		
		public CommandManager()
		{
			instance = this;
			server = Program.Get<NetworkServer>();
			if(server == null)
			{
				throw new Exception("Could not get 'NetworkServer' service.");
			}

			if(PlayerJoiningHook.isAvailable())
			{
				PlayerJoiningHook.registerCallback(new JoinMessage());
			}
			register(new HelpCommand());
		}

		private class JoinMessage : PlayerJoiningCallback
		{
			public void playerIsJoining(Connection connection, PlayerData playerData)
			{
				server.sendMessage(connection, "Try using '<color=#fa0>/help</color>' in chat.");
			}
		}

		private class HelpCommand : ICommand
		{
			public string name => "help";

			public string shortDescription => "Prints this list.";

			public void execute(CommandSender sender, string arguments)
			{
				//Print help message:
				StringBuilder builder = new StringBuilder();
				builder.Append("Available commands:");
				foreach(var command in instance.commands.Values)
				{
					builder
						.Append("\n- <color=#fa0>/")
						.Append(command.name)
						.Append("</color> : <color=#ccc>")
						.Append(command.shortDescription)
						.Append("</color>");
				}
				sender.sendMessage(builder.ToString());
			}
		}

		public void processChatEvent(ChatMessageEvent e)
		{
			if(e.isAlreadyRejected())
			{
				return;
			}

			String content = e.originalMessage.MessageContent;
			if(content.Length <= 1)
			{
				return;
			}
			if(content[0] != '/')
			{
				return;
			}
			content = content.Substring(1); //Dispose '/'
			e.result = MessageEventResult.Drop; //Yep, this is a command, no forwarding!
			CommandSender sender = new CommandSender(server, e.sender);
			
			(string command, string argument) split = splitArguments(content);

			commands.TryGetValue(split.command.ToLower(), out ICommand suitableCommand);
			if(suitableCommand == null)
			{
				sender.sendMessage("Unknown command '<color=#fa0>/" + split.command + "</color>'!");
				return;
			}
			suitableCommand.execute(sender, split.argument);
		}

		public static (string, string) splitArguments(string input)
		{
			String command;
			String arguments;
			int firstSpace = input.IndexOf(' ');
			if(firstSpace < 0)
			{
				command = input.Substring(0, input.Length).ToLower();
				arguments = null;
			}
			else
			{
				command = input.Substring(0, firstSpace).ToLower();
				arguments = input.Substring(firstSpace + 1, input.Length - firstSpace - 1);
			}
			return (command, arguments);
		}
	}
}
