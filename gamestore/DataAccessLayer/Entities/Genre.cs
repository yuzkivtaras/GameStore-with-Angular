using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Genre
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string? Name { get; set; }
        public string? ParentGenreId { get; set; }

        public virtual Genre? ParentGenre { get; set; }

        public virtual ICollection<GameGenre> GameGenres { get; set; } = new HashSet<GameGenre>();
    }
}
