using TwoRooms.Model;

namespace TwoRooms.Repository
{
	public class GameRepository : IGameRepository
	{
		//private AmazonDynamoDBClient DynamoDbClient;

		// public GameRepository(string accessKey, string secretKey, string url)
		// {
		// 	DynamoDbClient = new AmazonDynamoDBClient(
		// 		new BasicAWSCredentials(accessKey, secretKey),
		// 		new AmazonDynamoDBConfig { ServiceURL = url,
		// 			RegionEndpoint  = RegionEndpoint.USEast1});
		// }
		public void Add(Game game)
		{
			//https://stackoverflow.com/questions/55560495/how-to-put-items-in-dynamodb-using-c-sharp-lambda-functions-using-awssdk-dynamod
			
			//Table table = Table.LoadTable(DynamoDbClient, "GameCatalog");
		}
	}
}