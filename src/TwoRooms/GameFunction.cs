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
        private static readonly HttpClient httpClient = new HttpClient();
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
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await httpClient.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            return msg.Replace("\n","");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
			Console.WriteLine("Starting Game API Call!!!!!!");
			int statusCode = 200;
			var options = new JsonSerializerOptions
			{
				IgnoreNullValues = true,
			};
			Game game = new Game();

			try
			{
				Console.WriteLine("Begin json to Game object!!!!!!");
				game = JsonSerializer.Deserialize<Game>(apigProxyEvent.Body);
				game.GameName = "Mike" + new Random().Next().ToString();
				
				Console.WriteLine("Game number is " + game.NumberOfPlayers);
				Console.WriteLine("Begin write Game to DynamoDB");
				GameRepository.Put(game);
				return new APIGatewayProxyResponse
				{
					Body = JsonSerializer.Serialize(game, options),
					StatusCode = statusCode,
					Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
				};
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				statusCode = 500;
			}

			
			// Verify that we receive a Number of players in the request
			// create a new game object
			// Assign the Game name 
			// Initialise the player list
            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(game, options),
                StatusCode = statusCode,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
