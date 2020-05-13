using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using TwoRooms.Model;
using TwoRooms.Repository;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TwoRooms
{

    public class GameFunction
    {
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

            var msg = await HttpClient.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            return msg.Replace("\n","");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
			int statusCode = 200;
			Game game = new Game();

			try
			{
				game = apigProxyEvent.Body!=null ? JsonSerializer.Deserialize<Game>(apigProxyEvent.Body) : new Game();

				if (game.NumberOfPlayers == 0)
					return CreateApiGatewayProxyResponse(game, 400);

				game.GameName = "Mike" + new Random().Next(100).ToString();
				game.Players = new List<Player>();
				GameRepository.Put(game);
				return CreateApiGatewayProxyResponse(game, statusCode);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				statusCode = 500;
			}

			return CreateApiGatewayProxyResponse(game, statusCode);
        }

		private APIGatewayProxyResponse CreateApiGatewayProxyResponse(Game game,int statusCode)
		{
			return new APIGatewayProxyResponse
			{
				Body = JsonSerializer.Serialize(game, Options),
				StatusCode = statusCode,
				Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
			};
		}
	}
}
