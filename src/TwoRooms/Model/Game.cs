using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace TwoRooms.Model
{
	[DynamoDBTable("GameCatalog")]
	public class Game
	{
		[DynamoDBHashKey] 
		public string GameName { get; set; }
		public int NumberOfPlayers { get; set; }
		public List<Player> Players { get; set; }
		
	}
}