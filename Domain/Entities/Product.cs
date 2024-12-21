using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}