using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Order
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime OrderDate { get; set; }
        public string? CustomerId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? PaidDate { get; set; }

        public Customer? Customer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
