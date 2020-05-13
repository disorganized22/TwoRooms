using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using TwoRooms.Model;

namespace TwoRooms.Repository
{
	public class GameRepository : IGameRepository
	{
		private AmazonDynamoDBClient DynamoDbClient;
		private DynamoDBContext Context;

		public GameRepository()
		{
			DynamoDbClient = new AmazonDynamoDBClient();
			Context = new DynamoDBContext(DynamoDbClient);
		}
		
		public void Put(Game game)
		{
			try
			{
				if (game == null)
					return;

				Context.SaveAsync(game);
			}
			catch (AmazonDynamoDBException e)
			{
				Console.WriteLine(e.Message);
			}
			catch (AmazonServiceException e)
			{
				Console.WriteLine(e.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public Game Get(Game game)
		{
			Game result = Context.LoadAsync<Game>(game.GameName).Result;
			return result;
		}
	}
}