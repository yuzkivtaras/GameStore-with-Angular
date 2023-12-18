
namespace ServiceLayer.Models.Platform
{
    public class GetGameNameByPlatformDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? Name { get; set; }
    }
}
