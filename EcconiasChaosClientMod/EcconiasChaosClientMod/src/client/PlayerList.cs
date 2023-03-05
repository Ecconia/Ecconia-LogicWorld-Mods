using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LICC;
using LogicWorld.Players;

namespace EcconiasChaosClientMod.Client
{
	public static class PlayerList
	{
		private static readonly List<PlayerModel> players;

		static PlayerList()
		{
			var fieldPlayerList = typeof(PlayerModelsManager).GetField("WorldPlayerModels", BindingFlags.NonPublic | BindingFlags.Static);
			if(fieldPlayerList == null)
			{
				ModClass.logger.Error("Was not able to get the field 'WorldPlayerModels' in class 'PlayerModelsManager', the 'listplayer' commands will not be functional.");
				return;
			}
			var value = fieldPlayerList.GetValue(null);
			if(value == null)
			{
				ModClass.logger.Error("Value of field 'WorldPlayerModels' from class 'PlayerModelsManager' was 'null' this is unexpected, the 'listplayer' commands will not be functional.");
				return;
			}
			if(!(value is List<PlayerModel> playerModels))
			{
				ModClass.logger.Error("Value of field 'WorldPlayerModels' from class 'PlayerModelsManager' was of type '" + value.GetType() + "' this is unexpected, the 'listplayer' commands will not be functional.");
				return;
			}
			players = playerModels;
		}

		[Command("ListPlayersAll", Description = "Lists all players that are connected to this server. With all debug data.", Hidden = true)]
		public static void listPlayersAll()
		{
			if(players == null)
			{
				LConsole.WriteLine("Cannot use this command, as the player-model-list is not accessible. Check the logs for errors of this mod and report them to the maintainer of this mod.");
				return;
			}
			StringBuilder builder = new StringBuilder();
			builder.Append("All player models:");
			int amountOfPlayers = players.Count;
			int prefixWidth = amountOfPlayers.ToString().Length;
			for(int i = 0; i < amountOfPlayers; i++)
			{
				PlayerModel player = players[i];
				bool isYou = player == PlayerModelsManager.PlayerModelSelf;
				builder.Append('\n')
				       .Append(isYou ? '>' : '-')
				       .Append(' ');
				{
					string number = (i + 1).ToString();
					for(int j = number.Length; j < prefixWidth; j++)
					{
						builder.Append(' ');
					}
					builder.Append(number);
				}
				builder.Append(". ");
				if(player == null)
				{
					builder.Append("null");
					continue;
				}
				builder.Append(getPlayerName(player))
				       .Append(" <#")
				       .Append(player.Appearance.AppearanceData.BackgroundColor.ToStringWithoutPrefix())
				       .Append(">|BG|</color> <#")
				       .Append(player.Appearance.AppearanceData.ForegroundColor.ToStringWithoutPrefix())
				       .Append(">|FG|</color>");
			}
			LConsole.WriteLine(builder);
		}

		[Command("ListPlayers", Description = "Lists all players that are connected to this server.")]
		public static void listPlayers()
		{
			if(players == null)
			{
				LConsole.WriteLine("Cannot use this command, as the player-model-list is not accessible. Check the logs for errors of this mod and report them to the maintainer of this mod.");
				return;
			}
			StringBuilder builder = new StringBuilder();
			builder.Append("Visible players:");
			foreach(var player in players)
			{
				if(player == null)
				{
					continue;
				}
				bool isYou = player == PlayerModelsManager.PlayerModelSelf;
				builder.Append('\n')
				       .Append(isYou ? '>' : '-')
				       .Append(" <#")
				       .Append(player.Appearance.AppearanceData.BackgroundColor.ToStringWithoutPrefix())
				       .Append('>')
				       .Append(getPlayerName(player))
				       .Append("</color>");
			}
			LConsole.WriteLine(builder);
		}

		private static string getPlayerName(PlayerModel player)
		{
			string name = player.gameObject.name;
			if(name.StartsWith("Player Model: "))
			{
				name = name.Substring("Player Model: ".Length);
			}
			return name;
		}
	}
}
