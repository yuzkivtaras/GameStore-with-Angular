namespace ServiceLayer.Models.Game
{
    public class GameModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public int? UnitInStock { get; set; }
        public decimal? Price { get; set; }
        public int? Discontinued { get; set; }

        public List<string>? Genres { get; set; }
        public List<string>? Platforms { get; set; }
    }
}
