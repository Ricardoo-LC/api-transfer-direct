using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            ProductId = o.ProductId,
            Quantity = o.Quantity,
            Total = o.Total,
            Date = o.Date
        });
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var product = await _productRepository.GetByIdAsync(createOrderDto.ProductId);
            if (product == null) throw new KeyNotFoundException("Product not found");

            if (product.Stock < createOrderDto.Quantity)
            {
                throw new InvalidOperationException("Insufficient stock available");
            }

            var total = createOrderDto.Quantity * product.Price;

            var order = new Order
            {
                ProductId = createOrderDto.ProductId,
                Quantity = createOrderDto.Quantity,
                Total = total,
                Date = DateTime.UtcNow
            };

            product.Stock -= createOrderDto.Quantity;

            await _orderRepository.AddAsync(order);
            await _productRepository.UpdateAsync(product);
            
            await _unitOfWork.CommitTransactionAsync();

            return new OrderDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                Total = order.Total,
                Date = order.Date
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
