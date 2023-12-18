
namespace ServiceLayer.Models.Game
{
    public class GameResponseForDownloadDto
    {
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public int? UnitInStock { get; set; }
        public decimal? Price { get; set; }
        public int? Discontinued { get; set; }
        public string? FileName { get; set; }
        public byte[]? FileContent { get; set; }
    }
}
