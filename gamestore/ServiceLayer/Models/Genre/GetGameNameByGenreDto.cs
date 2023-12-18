
namespace ServiceLayer.Models.Genre
{
    public class GetGameNameByGenreDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? Name { get; set; }
    }
}
