using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace StoreAPI.Controllers;

[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // [HttpPut("game/{key}/buy")]
    // public IActionResult BuyGame()
    // {
    //    return Ok();
    // }

    // [HttpPut]
    // [Route("game/{id}/buy")]
    // public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
    // {
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    // var createOrder = await _orderService.CreateOrder(orderCreateDto);
    //    return CreatedAtAction(nameof(CreateOrder), createOrder);
    // }
    [HttpGet("orders")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orderModels = await _orderService.GetAllPaidOrdersAsync();

        return orderModels == null ? NotFound() : Ok(orderModels);
    }

    [HttpGet]
    [Route("orders/{id}")]
    public async Task<IActionResult> OrderDetails(string id)
    {
        var orderDetails = await _orderService.GetOrderByIdAsync(id);

        return orderDetails != null ? Ok(orderDetails) : NotFound();
    }

    [HttpGet]
    [Route("order/{id}")]
    public async Task<IActionResult> Order(string id)
    {
        var order = await _orderService.GetOrder(id);

        return order == null ? NotFound() : Ok(order);
    }
}
