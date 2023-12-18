using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Models.Order
{
    public class OrderDetailModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? ProductId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Discount { get; set; }

        public string? OrderId { get; set; }
        public OrderModel? OrderModel { get; set; }
    }
}
