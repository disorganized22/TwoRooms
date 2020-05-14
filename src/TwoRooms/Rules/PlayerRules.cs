using System;
using System.Collections.Generic;
using System.Linq;
using TwoRooms.Model;

namespace TwoRooms.Rules
{
	public class PlayerRules
	{
		public Player PopulatePlayer(Game game, string playerName)
		{
			Player player = game.Players.Find(p => p.Name == playerName);
			if (player != null)
				return player;

			player = new Player {Number = game.Players.Count + 1, Name = playerName, Color = GetColor(game)};
			player.Role = GetRole(game, player.Color);
			player.Group = GetGroup(game);
			
			game.Players.Add(player);
			

			return player;
		}

		private string GetRole(Game game, string color)
		{
			List<string> roles = new List<string>();

			if (color == "NONE")
				return "GAMBLER";
			string mainrole = color == "RED" ? "BOMBER" : "PRESIDENT";
			if (game.Players.Count(p => p.Color == color && p.Role == mainrole) == 0)
				roles.Add(mainrole);
			int colorCount = game.Players.Count(p => p.Color == color && p.Role == "PLAYER");
			if (colorCount < game.NumberOfPlayers/2 -1)
				roles.Add("PLAYER");

			var random = new Random();
			int index = random.Next(roles.Count);
			return roles[index];
			
		}
		private string GetColor(Game game)
		{
			
			List<string> colors = new List<string>();
			int redCount = game.Players.Count(p => p.Color == "RED");
			int blueCount = game.Players.Count(p => p.Color == "BLUE");
			if (game.NumberOfPlayers/2 >blueCount)
				colors.Add("BLUE");
			if (game.NumberOfPlayers/2 >redCount)
				colors.Add("RED");
			if (game.NumberOfPlayers % 2 > 0 && game.Players.All(p => p.Role != "GAMBLER"))
				colors.Add("NONE");
			
			var random = new Random();
			int index = random.Next(colors.Count);
			
			return colors[index];
		}
		private int GetGroup(Game game)
		{
			List<int> groups = new List<int>();
			int oneCount = game.Players.Count(p => p.Group == 1);
			int twoCount = game.Players.Count(p => p.Group == 2);
			int numberToCheck = game.NumberOfPlayers / 2 + game.NumberOfPlayers % 2;
			if (numberToCheck > oneCount)
				groups.Add(1);
			if (numberToCheck > twoCount)
				groups.Add(2);
			var random = new Random();
			int index = random.Next(groups.Count);
			
			return groups[index];
		}

	}
}