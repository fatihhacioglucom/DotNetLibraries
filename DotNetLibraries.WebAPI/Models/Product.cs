namespace DotNetLibraries.WebAPI.Models;

public sealed class Product
{
    
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }

    public Product()
    {
        Id = Guid.CreateVersion7();
    }
}