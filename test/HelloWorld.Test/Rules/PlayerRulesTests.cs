using System.Collections.Generic;
using NUnit.Framework;
using TwoRooms.Model;
using TwoRooms.Rules;
using NUnit.Framework;

namespace TwoRooms.Tests.Rules
{
	public class PlayerRulesTests
	{
		private PlayerRules TestSubject;
		private Player RequestPlayer;
		private Player ExpectPlayer;

		[SetUp]
		public void SetUp()
		{
			TestSubject = new PlayerRules();
			RequestPlayer = new Player();
			RequestPlayer.Name = "TwoRooms";
			ExpectPlayer = new Player();
			ExpectPlayer.Name = "TwoRooms";
			ExpectPlayer.Color = PlayerColors.RED.ToString();
			ExpectPlayer.Role = PlayerRoles.PLAYER.ToString();
			ExpectPlayer.Number = 5;
			ExpectPlayer.Group = 2;
		}

		[Test]
		public void PopulatePlayer_WithExistingPlayer_ReturnsExistingPlayer()
		{
			Game game = new Game();
			game.Players = new List<Player>(){ExpectPlayer};
			Player result = TestSubject.PopulatePlayer(game, "TwoRooms");
			Assert.That(result, Is.EqualTo(ExpectPlayer));
			Assert.That(result.Color, Is.EqualTo("RED"));
			Assert.That(result.Role, Is.EqualTo("PLAYER"));
			Assert.That(result.Number, Is.EqualTo(5));
			Assert.That(result.Group, Is.EqualTo(2));
		}
		
	}
}