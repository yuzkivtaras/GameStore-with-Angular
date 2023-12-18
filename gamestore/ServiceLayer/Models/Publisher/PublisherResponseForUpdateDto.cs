
namespace ServiceLayer.Models.Publisher
{
    public class PublisherResponseForUpdateDto
    {
        public string? Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HomePage { get; set; } = string.Empty;
    }
}
