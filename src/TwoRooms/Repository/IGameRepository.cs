using TwoRooms.Model;

namespace TwoRooms.Repository
{
	public interface IGameRepository
	{
		public void Put(Game game);
		public Game Get(Game game);
	}
}