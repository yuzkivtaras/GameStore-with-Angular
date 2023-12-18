using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Game
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string? Name { get; set; }
        [Key]
        public string? Key { get; set; }
        public string? Description { get; set; }
        [Required]
        public int? UnitInStock { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public int? Discontinued { get; set; }

        public string? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public virtual ICollection<GameGenre> GameGenres { get; set; } = new HashSet<GameGenre>();

        public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new HashSet<GamePlatform>();
    }
}
