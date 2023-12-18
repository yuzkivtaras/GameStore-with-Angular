
namespace ServiceLayer.Models.Game
{
    public class GetPlatformNameByGameDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Type { get; set; }
    }
}
