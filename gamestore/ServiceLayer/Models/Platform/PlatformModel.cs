
namespace ServiceLayer.Models
{
    public class PlatformModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Type { get; set; }
    }
}
