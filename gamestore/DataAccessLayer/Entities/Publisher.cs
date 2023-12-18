
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Publisher
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string? CompanyName { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? HomePage { get; set; }

        public ICollection<Game>? Games { get; set; }
    }
}
