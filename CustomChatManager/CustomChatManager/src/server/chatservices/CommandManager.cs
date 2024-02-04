using System;
using System.Collections.Generic;
using System.Text;
using CustomChatManager.Server.Commands;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Server.Hooks;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicAPI.Networking.Packets.Server;
using LogicAPI.Server.Networking;
using LogicLog;

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
		
		private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
		
		public void register(ICommand command)
		{
			commands.Add(command.name.ToLower(), command);
		}
		
		public CommandManager(ILogicLogger logger)
		{
			instance = this;
			server = ServiceGetter.getService<NetworkServer>();
			
			try
			{
				PlayerJoiningHook.registerCallback(new JoinMessage());
			}
			catch(Exception e)
			{
				logger.Error("Could not register player join callback. See stacktrace:\n" + e);
			}
			register(new HelpCommand());
		}
		
		private class JoinMessage : PlayerJoiningHook.PlayerJoiningCallback
		{
			public void playerIsJoining(Connection connection, PlayerData playerData)
			{
				//The 'h' in 'help' is intentionally lowercase: It looks better and it might give users the idea, that casing in commands does not matter.
				server.sendMessage(connection, "Try using '" + ChatColors.highlight + "/help" + ChatColors.close + "' in chat.");
			}
		}
		
		private class HelpCommand : ICommand
		{
			public string name => "Help";
			
			public string shortDescription => "Prints this list.";
			
			public void execute(CommandSender sender, string arguments)
			{
				//Print help message:
				var builder = new StringBuilder();
				builder.Append("Available commands:");
				foreach(var command in instance.commands.Values)
				{
					builder
						.Append("\n- ")
						.Append(ChatColors.highlight)
						.Append('/')
						.Append(command.name)
						.Append(ChatColors.close)
						.Append(" : ")
						.Append(ChatColors.background)
						.Append(command.shortDescription)
						.Append(ChatColors.close);
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
			
			var content = e.originalMessage.MessageContent;
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
			var sender = new CommandSender(server, e.sender);
			
			(string command, string argument) split = splitArguments(content);
			
			commands.TryGetValue(split.command.ToLower(), out var suitableCommand);
			if(suitableCommand == null)
			{
				sender.sendMessage(ChatColors.failure + "Unknown command '" + ChatColors.highlight + "/" + split.command + ChatColors.close + "'!" + ChatColors.close);
				return;
			}
			suitableCommand.execute(sender, split.argument);
		}
		
		public static (string, string) splitArguments(string input)
		{
			string command;
			string arguments;
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
