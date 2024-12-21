using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _orderService = new OrderService(
            _mockOrderRepository.Object,
            _mockProductRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenStockIsSufficient()
    {
        var productId = 1;
        var createOrderDto = new CreateOrderDto
        {
            ProductId = productId,
            Quantity = 2
        };

        var product = new Product
        {
            Id = productId,
            Stock = 10,
            Price = 50.0m
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockOrderRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _mockProductRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(uow => uow.CommitTransactionAsync())
            .Returns(Task.CompletedTask);
        
        var result = await _orderService.CreateOrderAsync(createOrderDto);
        
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(100.0m, result.Total);

        _mockOrderRepository.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenProductNotFound()
    {
        var productId = 2;
        var createOrderDto = new CreateOrderDto
        {
            ProductId = productId,
            Quantity = 4
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product)null!); // Producto no encontrado
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.CreateOrderAsync(createOrderDto));

        _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.RollbackTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenStockIsInsufficient()
    {
        var productId = 3;
        var createOrderDto = new CreateOrderDto
        {
            ProductId = productId,
            Quantity = 10
        };

        var product = new Product
        {
            Id = productId,
            Stock = 2,
            Price = 50.0m
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(createOrderDto));

        _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.RollbackTransactionAsync(), Times.Once);
    }
}
