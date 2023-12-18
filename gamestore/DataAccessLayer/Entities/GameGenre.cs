
namespace DataAccessLayer.Entities
{
    public class GameGenre
    {
        public string? GamesKey { get; set; }
        public Game? Game { get; set; }

        public string? GenresId { get; set; }
        public Genre? Genre { get; set; }
    }
}
