using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Moq;
using TwoRooms.Model;
using TwoRooms.Repository;

namespace TwoRooms.Tests
{
  public class GameFunctionTest
  {
    private static readonly HttpClient client = new HttpClient();

    private static async Task<string> GetCallingIP()
    {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var stringTask = client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            var msg = await stringTask;
            return msg.Replace("\n","");
    }

    [Fact]
    public async Task TestHelloWorldFunctionHandler()
    {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();
            string location = GetCallingIP().Result;
			Dictionary<string, object> reqbody = new Dictionary<string, object>
			{
				{ "NumberOfPlayers", 6 },
			};
			request.Body = JsonConvert.SerializeObject(reqbody);
            Dictionary<string, object> body = new Dictionary<string, object>
            {
				{ "NumberOfPlayers", 6 },
            };

            var expectedResponse = new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

			var mockRepo = new Mock<IGameRepository>();
			mockRepo.Setup(repo => repo.Put(It.IsAny<Game>()));
			var function = new GameFunction(mockRepo.Object);
            var response = await function.FunctionHandler(request, context);
			Game game =  System.Text.Json.JsonSerializer.Deserialize<Game>(response.Body);
			Assert.Equal(game.NumberOfPlayers, 6);
            Assert.Equal(expectedResponse.Headers, response.Headers);
            Assert.Equal(expectedResponse.StatusCode, response.StatusCode);
    }
  }
}