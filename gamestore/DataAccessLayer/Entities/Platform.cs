using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Platform
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string? Type { get; set; }

        public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new HashSet<GamePlatform>();
    }
}
