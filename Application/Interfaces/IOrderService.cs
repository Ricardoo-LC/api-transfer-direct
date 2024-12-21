using Application.DTOs;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
}