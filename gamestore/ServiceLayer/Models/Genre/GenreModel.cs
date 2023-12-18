
namespace ServiceLayer.Models.Genre
{
    public class GenreModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? ParentGenreId { get; set; }

        public virtual GenreModel? ParentGenre { get; set; }
    }
}
