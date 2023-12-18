using ServiceLayer.Models.Order;

namespace ServiceLayer.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderModelResult>> GetAllPaidOrdersAsync();

        Task<List<OrderDetailModel>?> GetOrderByIdAsync(string id);

        Task<IEnumerable<BasketOrders?>> GetBasketOrders();

        Task<OrderModel?> GetOrder(string id);

        //Task<OrderResponseDto> CreateOrder(OrderCreateDto orderCreateDto);
    }
}
