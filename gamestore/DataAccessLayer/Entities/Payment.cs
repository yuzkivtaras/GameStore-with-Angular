using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Payment
    {
        public string? ImageUrl { get; set; }
        [Key]
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
