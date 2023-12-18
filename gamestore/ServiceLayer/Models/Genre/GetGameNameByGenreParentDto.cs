
namespace ServiceLayer.Models.Genre
{
    public class GetGameNameByGenreParentDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? ParentId { get; set; }
        public string? Name { get; set; }
    }
}
