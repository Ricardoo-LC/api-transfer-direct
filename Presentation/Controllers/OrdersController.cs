using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly JwtService _jwtService;

    public OrdersController(IOrderService orderService, JwtService jwtService)
    {
        _orderService = orderService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var token = _jwtService.GenerateToken();
        
        Response.Headers.Add("X-JWT-Token", token);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdOrder = await _orderService.CreateOrderAsync(orderDto);
        var token = _jwtService.GenerateToken();
        
        Response.Headers.Add("X-JWT-Token", token);
        
        return CreatedAtAction(nameof(GetAllOrders), new { id = createdOrder.Id }, new
        {
            Order = createdOrder,
            Token = token
        });
    }
}
