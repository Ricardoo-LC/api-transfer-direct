using Domain.Entities;

namespace Application.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task AddAsync(Order order);
}