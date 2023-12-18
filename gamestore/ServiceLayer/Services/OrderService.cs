using AutoMapper;
using DataAccessLayer.Interfaces;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Order;

namespace ServiceLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderModelResult>> GetAllPaidOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            var paidOrders = orders.Select(o =>
                new OrderModelResult
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate,
                    OrderDetails = o.OrderDetails!.Select(d => new OrderDetailModel
                    {
                        Id = d.Id,
                        ProductId = d.ProductId,
                        Price = d.Price,
                        Quantity = d.Quantity,
                        Discount = d.Discount,
                    }).ToList()
                });
            return paidOrders;
        }

        public async Task<List<OrderDetailModel>?> GetOrderByIdAsync(string id)
        {
            var orderEntity = await _unitOfWork.OrderRepository.GetOrder(id);

            if (orderEntity != null)
            {
                var orderDetails = orderEntity.OrderDetails!.Select(d => new OrderDetailModel
                        {
                            Id = d.Id,
                            ProductId = d.ProductId,
                            Price = d.Price,
                            Quantity = d.Quantity,
                            Discount = d.Discount,
                        }).ToList();

                return orderDetails;
            }
            else
            {
                return null;
            }
        }

        public async Task<OrderModel?> GetOrder(string id)
        {
            var orderEntity = await _unitOfWork.OrderRepository.GetOrder(id);

            if (orderEntity != null)
            {
                var order = new OrderModel
                {
                    Id = orderEntity.Id,
                    CustomerId = orderEntity.CustomerId,
                    OrderDate = orderEntity.OrderDate
                };

                return order;
            }

            return null;
        }

        public async Task<IEnumerable<BasketOrders?>> GetBasketOrders()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllForBasketAsync();

            return orders.Where(o => o.OrderDetails != null && o.OrderDetails.Any())
                .Select(order => new BasketOrders
                {
                    Id = order.Id,
                    ProductId = order.OrderDetails!.First().ProductId,
                    ProductName = order.OrderDetails!.First().ProductName,
                    Sum = order.OrderDetails!.First().Sum,
                    Price = order.OrderDetails!.First().Price,
                    Quantity = order.OrderDetails!.First().Quantity,
                    Discount = order.OrderDetails!.First().Discount
                });
        }

        //public async Task<OrderResponseDto> CreateOrder(OrderCreateDto orderCreateDto)
        //{
        //    var orderDetails = new List<OrderDetail>();

        //    foreach (var productId in orderCreateDto.OrderDetails)
        //    {
        //        var product = await _unitOfWork.GameRepository.GetGameDetailsByKeyAsync(productId);

        //        if (product == null) throw new Exception("Product not found");

        //        var orderDetail = new OrderDetail
        //        {
        //            ProductId = productId,
        //            Price = (decimal)product.Price,
        //            Quantity = 1 
        //        };

        //        orderDetails.Add(orderDetail);
        //    }

        //    var order = new Order
        //    {
        //        OrderDate = DateTime.Now,
        //        OrderDetails = orderDetails,
        //        CustomerId = orderCreateDto.CustomerId,
        //        Price = orderDetails.Sum(d => d.Price),
        //        Discount = 0 
        //    };
        //    order.Sum = order.Price - (order.Price * order.Discount / 100);

        //    var createdOrder = await _unitOfWork.OrderRepository.CreateOrder(order);

        //    return new OrderResponseDto
        //    {
        //        CustomerId = createdOrder.CustomerId,
        //        OrderDate = createdOrder.OrderDate,
        //        Sum = createdOrder.Sum,
        //        CreationDate = createdOrder.CreationDate,
        //        Price = createdOrder.Price,
        //        Discount = createdOrder.Discount,
        //        PaidDate = createdOrder.PaidDate
        //    };
        //}
    }
}
