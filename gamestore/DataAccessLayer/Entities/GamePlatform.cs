
namespace DataAccessLayer.Entities
{
    public class GamePlatform
    {
        public string? GamesKey { get; set; }
        public Game? Game { get; set; }

        public string? PlatformsId { get; set; }
        public Platform? Platform { get; set; }
    }
}
