using System;
using System.Linq;
using Amazon;
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

		public GameRepository()
		{
			DynamoDbClient = new AmazonDynamoDBClient();
			DynamoDBContext context = new DynamoDBContext(DynamoDbClient);
		}
		// public GameRepository(string accessKey, string secretKey, string url)
		// {
		// 	DynamoDbClient = new AmazonDynamoDBClient(
		// 		new BasicAWSCredentials(accessKey, secretKey),
		// 		new AmazonDynamoDBConfig { ServiceURL = url,
		// 			RegionEndpoint  = RegionEndpoint.USEast1});
		// }
		public void Put(Game game)
		{
			Console.WriteLine("Starting Add Method");
			//https://stackoverflow.com/questions/55560495/how-to-put-items-in-dynamodb-using-c-sharp-lambda-functions-using-awssdk-dynamod
			try {
				Console.WriteLine("Starting Add Method");

			Table table = Table.LoadTable(DynamoDbClient, "GameCatalog");
			Console.WriteLine("Created Table link");
			if (game == null) 
				return;

			Console.WriteLine("Create document");

			var doc = new Document();
			doc["GameName"] = game.GameName;
			doc["NumberOfPlayers"] = game.NumberOfPlayers;
			var result = table.PutItemAsync(doc).Result;
			Console.WriteLine("wrote document");

			}
			catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
			catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
			catch (Exception e) { Console.WriteLine(e.Message); }
		}

		public Game Get(Game game)
		{
			throw new NotImplementedException();
		}
	}
}