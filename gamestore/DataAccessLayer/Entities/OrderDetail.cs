

namespace DataAccessLayer.Entities
{
    public class OrderDetail
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? ProductId { get; set; }

        public string? ProductName { get; set; }

        public decimal Sum { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Discount { get; set; }

        public string? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
