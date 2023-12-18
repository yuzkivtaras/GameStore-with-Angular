using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly GameStoreDbContext _dbContext;

        public OrderRepository(GameStoreDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = await _dbContext.Orders
                .Where(o => o.PaidDate != null)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.OrderDetails = order.OrderDetails ?? new List<OrderDetail>();
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetAllForBasketAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.PaidDate == null)
                .ToListAsync();
        }

        public async Task<Order?> GetOrder(string id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id != null && o.Id == id);

            return order;
        }
    }
}
