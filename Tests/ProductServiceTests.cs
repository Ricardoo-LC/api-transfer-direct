using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockProductRepository.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 10.0m, Stock = 5 },
            new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 20.0m, Stock = 10 }
        };

        _mockProductRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(products);

        var result = await _productService.GetAllProductsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Product 1", result.First().Name);

        _mockProductRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var productId = 1;
        var product = new Product
        {
            Id = productId,
            Name = "Product 1",
            Description = "Description 1",
            Price = 10.0m,
            Stock = 5
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var result = await _productService.GetProductByIdAsync(productId);

        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Product 1", result.Name);

        _mockProductRepository.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldThrowException_WhenProductNotFound()
    {
        var productId = 99;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product)null!);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductByIdAsync(productId));

        _mockProductRepository.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task AddProductAsync_ShouldAddProductAndReturnDto()
    {
        var createDto = new CreateUpdateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 30.0m,
            Stock = 15
        };

        var product = new Product
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Stock = createDto.Stock
        };

        _mockProductRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => p.Id = product.Id) // Simular asignación de Id
            .Returns(Task.CompletedTask);

        var result = await _productService.AddProductAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(createDto.Name, result.Name);

        _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateProductAndReturnDto_WhenProductExists()
    {
        var productId = 1;
        var updateDto = new CreateUpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 40.0m,
            Stock = 20
        };

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Product",
            Description = "Old Description",
            Price = 10.0m,
            Stock = 5
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _productService.UpdateProductAsync(productId, updateDto);

        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal(updateDto.Name, result.Name);

        _mockProductRepository.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
        _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound()
    {
        var productId = 99;
        var updateDto = new CreateUpdateProductDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 40.0m,
            Stock = 20
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product)null!);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.UpdateProductAsync(productId, updateDto));

        _mockProductRepository.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
    {
        var productId = 1;
        var product = new Product
        {
            Id = productId,
            Name = "Product to Delete",
            Description = "Description",
            Price = 20.0m,
            Stock = 10
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockProductRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        await _productService.DeleteProductAsync(productId);

        _mockProductRepository.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
        _mockProductRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldThrowException_WhenProductNotFound()
    {
        var productId = 99;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product)null!);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.DeleteProductAsync(productId));

        _mockProductRepository.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
    }
}