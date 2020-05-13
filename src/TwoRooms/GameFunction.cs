using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using TwoRooms.Model;
using TwoRooms.Repository;
using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace TwoRooms
{
	public class GameFunction
	{
		public static readonly string GAME_PREFIX = "Game";
		private static readonly HttpClient HttpClient = new HttpClient();

		private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
		{
			IgnoreNullValues = true,
		};

		private IGameRepository GameRepository;

		public GameFunction()
		{
			GameRepository = new GameRepository();
		}

		public GameFunction(IGameRepository repository)
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
			int statusCode = 200;
			Game game = new Game();

			try
			{
				game = apigProxyEvent.Body != null ? System.Text.Json.JsonSerializer.Deserialize<Game>(apigProxyEvent.Body) : new Game();

				if (game.NumberOfPlayers == 0)
					return CreateApiGatewayProxyResponse(game, 400);
				int counter = 0;
				while (game.GameName == null && counter < 5)
				{
					counter++;
					game.GameName = GAME_PREFIX + new Random().Next(1000).ToString();
					if (GameRepository.Get(game) != null)
						game.GameName = null;
				}

				game.Players = new List<Player>();
				GameRepository.Put(game);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				statusCode = 500;
			}

			return CreateApiGatewayProxyResponse(game, statusCode);
		}

		private APIGatewayProxyResponse CreateApiGatewayProxyResponse(Game game, int statusCode)
		{
			return new APIGatewayProxyResponse
			{
				Body = System.Text.Json.JsonSerializer.Serialize(game, Options),
				StatusCode = statusCode,
				Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
			};
		}
	}
}