using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Order order);

        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetAllForBasketAsync();

        Task<Order?> GetOrder(string id);
    }
}
