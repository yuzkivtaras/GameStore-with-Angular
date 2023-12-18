using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace StoreAPI.Controllers;

[ApiController]
public class BasketController : ControllerBase
{
    private readonly IOrderService _orderService;

    public BasketController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orderModels = await _orderService.GetBasketOrders();

        return orderModels == null ? NotFound() : Ok(orderModels);
    }
}
