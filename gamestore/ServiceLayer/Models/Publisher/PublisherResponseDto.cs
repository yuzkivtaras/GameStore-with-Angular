
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Models.Publisher
{
    public class PublisherResponseDto
    {
        [Required]
        public string? CompanyName { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? HomePage { get; set; }
    }
}
