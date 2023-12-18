using ServiceLayer.Models.Genre;


namespace ServiceLayer.Models.Game
{
    public class GameCreateDto
    {
        public GameResponseDto? Game { get; set; }
        public List<string> Genres { get; set; } = new();
        public List<string> Platforms { get; set; } = new();
        public string Publisher { get; set; } = default!;
    }
}
