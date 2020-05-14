using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TwoRooms.Model;
using TwoRooms.Repository;
using TwoRooms.Rules;
using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace Players
{
	public class PlayerFunction
	{
		private static readonly HttpClient HttpClient = new HttpClient();

		private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
		{
			IgnoreNullValues = true,
		};

		private IGameRepository GameRepository;

		public PlayerFunction()
		{
			GameRepository = new GameRepository();
		}

		public PlayerFunction(IGameRepository repository)
		{
			GameRepository = repository;
		}

		private static async Task<string> GetCallingIP()
		{
			HttpClient.DefaultRequestHeaders.Accept.Clear();
			HttpClient.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

			var msg = await HttpClient.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext: false);

			return msg.Replace("\n", "");
		}

		public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
		{
			Console.WriteLine("Starting method "+ DateTime.Now);
			int statusCode = 200;
			Player player = new Player();
			Game game =new Game();

			string gamename = apigProxyEvent.PathParameters["gamename"];
			Console.WriteLine("Game Name is  "+gamename);

			try
			{
				player = apigProxyEvent.Body != null ? System.Text.Json.JsonSerializer.Deserialize<Player>(apigProxyEvent.Body) : new Player();
				if (gamename != null)
				{
					Console.WriteLine("Calling Repo Get.");
					game = GameRepository.Get(new Game() {GameName = gamename});
					if (game.Players.Count == game.NumberOfPlayers)
					{
						player.Name = "GAME IS FULL";
						return CreateApiGatewayProxyResponse(player, statusCode);
					}

					Console.WriteLine("Got back from Get.");
					player = new PlayerRules().PopulatePlayer(game, player.Name);
					GameRepository.Put(game);
					Console.WriteLine("Got back from Put.");
				}
				
				// Validation do I have a gameName
				// Are there available player spots
				// Does a player with that name already exist?
				//
				// Get Game
				// Run validation
				// Create Player with all rules.
				// Add player to Game and save game
				// Return Player

			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				statusCode = 500;
			}
			Console.WriteLine("Ending method "+ DateTime.Now);

			return CreateApiGatewayProxyResponse(player, statusCode);
		}

		private APIGatewayProxyResponse CreateApiGatewayProxyResponse(Player player, int statusCode)
		{
			return new APIGatewayProxyResponse
			{
				Body = System.Text.Json.JsonSerializer.Serialize(player, Options),
				StatusCode = statusCode,
				Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
			};
		}
	}
}