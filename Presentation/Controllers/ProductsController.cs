using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly JwtService _jwtService;

    public ProductsController(IProductService productService, JwtService jwtService)
    {
        _productService = productService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        var token = _jwtService.GenerateToken();
        
        Response.Headers.Add("X-JWT-Token", token);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            var token = _jwtService.GenerateToken();
            
            Response.Headers.Add("X-JWT-Token", token);
            return Ok(new
            {
                Product = product,
                Token = token
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Product not found");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateUpdateProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduct = await _productService.AddProductAsync(productDto);
        var token = _jwtService.GenerateToken();
        
        Response.Headers.Add("X-JWT-Token", token);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, new
        {
            Product = createdProduct,
            Token = token
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, CreateUpdateProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
            var token = _jwtService.GenerateToken();
            Response.Headers.Add("X-JWT-Token", token);
            return Ok(new
            {
                Product = updatedProduct,
                Token = token
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Product not found");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Product not found");
        }
    }
}
