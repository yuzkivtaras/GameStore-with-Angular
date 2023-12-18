
namespace ServiceLayer.Models.Genre
{
    public class GenreResponseForUpdateDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? ParentGenreId { get; set; }
    }
}
