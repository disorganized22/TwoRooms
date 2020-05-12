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
		private string AccessKey;
		private string SecretKey;
		private string ServiceUrl;
		private IGameRepository GameRepository;

		public GameFunction()
		{
			// AccessKey = Environment.GetEnvironmentVariable("AccessKey");
			// SecretKey = Environment.GetEnvironmentVariable("SecretKey");
			// ServiceUrl = Environment.GetEnvironmentVariable("ServiceURL");
			// GameRepository = new GameRepository(AccessKey, SecretKey, ServiceUrl);
		}

		public GameFunction(IGameRepository repository)
		{
			GameRepository = repository;
			AccessKey = Environment.GetEnvironmentVariable("AccessKey");
			SecretKey = Environment.GetEnvironmentVariable("SecretKey");
			ServiceUrl = Environment.GetEnvironmentVariable("ServiceURL");
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
			int statusCode = 200;
            var location = await GetCallingIP();
			var options = new JsonSerializerOptions
			{
				IgnoreNullValues = true,
			};
			Game game = new Game();

			try
			{
				game = JsonSerializer.Deserialize<Game>(apigProxyEvent.Body);
				
				Console.WriteLine("Game number is " + game.NumberOfPlayers);
				//GameRepository.Add(game);
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
