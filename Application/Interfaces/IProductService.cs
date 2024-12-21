using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<ProductDto> AddProductAsync(CreateUpdateProductDto productDto);
    Task<ProductDto> UpdateProductAsync(int id, CreateUpdateProductDto productDto);
    Task DeleteProductAsync(int id);
}