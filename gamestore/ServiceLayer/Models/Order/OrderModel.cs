
using DataAccessLayer.Entities;

namespace ServiceLayer.Models.Order
{
    public class OrderModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime OrderDate { get; set; }
        public string? CustomerId { get; set; }
        //public ICollection<OrderDetailModel>? OrderDetails { get; set; } = new List<OrderDetailModel>();
    }

    public class OrderModelResult
    {
        //public OrderModel? Orders { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime OrderDate { get; set; }
        public string? CustomerId { get; set; }
        public List<OrderDetailModel>? OrderDetails { get; set; } = new List<OrderDetailModel>();
    }
}
