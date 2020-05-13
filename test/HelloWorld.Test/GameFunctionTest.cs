using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using TwoRooms.Model;
using TwoRooms.Repository;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TwoRooms.Tests
{
	public class GameFunctionTest
	{
		private static readonly HttpClient client = new HttpClient();
		private IGameRepository GameRepository;
		private GameFunction TestSubject;
		private Dictionary<string, string> Headers;
		private APIGatewayProxyRequest Request;
		private TestLambdaContext Context;

		[SetUp]
		public void SetUp()
		{
			GameRepository = Substitute.For<IGameRepository>();
			TestSubject = new GameFunction(GameRepository);
			Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};
			Request = new APIGatewayProxyRequest();
			Context = new TestLambdaContext();

			GameRepository.Get(new Game()).ReturnsNullForAnyArgs();
			Dictionary<string, object> reqbody = new Dictionary<string, object>
			{
				{"NumberOfPlayers", 6},
			};
			Request.Body = JsonConvert.SerializeObject(reqbody);
		}

		[Test]
		public async Task FunctionHandler_WithRequestAndContext_ReturnsStatusCode()
		{
			var response = await TestSubject.FunctionHandler(Request, Context);
			Assert.That(response.StatusCode, Is.EqualTo(200));
		}

		[Test]
		public async Task FunctionHandler_WithRequestAndContext_ReturnsProperHeaders()
		{
			var response = await TestSubject.FunctionHandler(Request, Context);
			Assert.That(response.Headers, Is.EqualTo(Headers));
		}

		[Test]
		public async Task FunctionHandler_WithRequestAndContext_ReturnsNewGameName()
		{
			var response = await TestSubject.FunctionHandler(Request, Context);
			Game game = JsonSerializer.Deserialize<Game>(response.Body);
			Assert.That(game.GameName.StartsWith(GameFunction.GAME_PREFIX));
		}

		[Test]
		public async Task FunctionHandler_WithoutPlayersEntered_Returns400BadRequest()
		{
			Request.Body = "{}";
			var response = await TestSubject.FunctionHandler(Request, Context);
			Assert.That(response.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task FunctionHandler_WithoutRequestBody_Returns400BadRequest()
		{
			Request.Body = null;
			var response = await TestSubject.FunctionHandler(Request, Context);
			Assert.That(response.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public async Task FunctionHandler_WithException_Returns500StatusCode()
		{
			Request.Body = "{NateSuxz";
			var response = await TestSubject.FunctionHandler(Request, Context);
			Assert.That(response.StatusCode, Is.EqualTo(500));
		}
	}
}