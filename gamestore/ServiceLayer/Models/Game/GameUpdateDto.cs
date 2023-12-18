
namespace ServiceLayer.Models.Game
{
    public class GameUpdateDto
    {
        public GameResponseForUpdateDto? Game {  get; set; }
        public List<string>? Genres { get; set; }
        public List<string>? Platforms { get; set; }
        public string? Publisher { get; set; }
    }
}
