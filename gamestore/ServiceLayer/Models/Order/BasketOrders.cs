

namespace ServiceLayer.Models.Order
{
    public class BasketOrders
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Sum { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
