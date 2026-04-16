namespace backend.Dtos;

public class ProductListItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int? BrandId { get; set; }

    public string BrandName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }

    public int Stock { get; set; }
}
