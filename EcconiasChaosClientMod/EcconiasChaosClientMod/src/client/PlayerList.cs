using System.Collections.Generic;
using System.Text;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicAPI.Data;
using LogicWorld.Players;

namespace EcconiasChaosClientMod.Client
{
	public static class PlayerList
	{
		private static readonly Dictionary<ShortPlayerID, PlayerModel> players;
		
		static PlayerList()
		{
			var fieldPlayerList = Fields.getPrivateStatic(typeof(PlayerModelsManager), "WorldPlayerModels");
			var value = Fields.getNonNull(fieldPlayerList, "Value of field 'WorldPlayerModels' from class 'PlayerModelsManager' was 'null'.");
			var playerModels = Types.checkType<Dictionary<ShortPlayerID, PlayerModel>>(value);
			players = playerModels;
		}
		
		[Command("ListPlayersAll", Description = "Lists all players that are connected to this server. With all debug data.", Hidden = true)]
		public static void listPlayersAll()
		{
			var builder = new StringBuilder();
			builder.Append("All player models:");
			var playerList = new List<PlayerModel>(players.Values);
			var amountOfPlayers = playerList.Count;
			var prefixWidth = amountOfPlayers.ToString().Length;
			for(var i = 0; i < amountOfPlayers; i++)
			{
				var player = playerList[i];
				var isYou = player == PlayerModelsManager.PlayerModelSelf;
				builder.Append('\n')
				       .Append(isYou ? '>' : '-')
				       .Append(' ');
				{
					var number = (i + 1).ToString();
					for(var j = number.Length; j < prefixWidth; j++)
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
			var builder = new StringBuilder();
			builder.Append("Visible players:");
			foreach(var player in players.Values)
			{
				if(player == null)
				{
					continue;
				}
				var isYou = player == PlayerModelsManager.PlayerModelSelf;
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
			var name = player.gameObject.name;
			if(name.StartsWith("Player Model: "))
			{
				name = name.Substring("Player Model: ".Length);
			}
			return name;
		}
	}
}
