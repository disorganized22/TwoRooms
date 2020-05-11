using System.Collections.Generic;

namespace TwoRooms.Model
{
	public class Game
	{
		public string GameName { get; set; }
		public int NumberOfPlayers { get; set; }
		public List<Player> Players { get; set; }
		
	}
}