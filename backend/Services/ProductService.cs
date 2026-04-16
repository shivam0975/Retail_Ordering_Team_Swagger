using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ProductService(RetailOrderingContext context) : IProductService
{
    public async Task<List<ProductListItemDto>> GetProductsAsync(string? search, int? categoryId)
    {
        var query = context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Inventory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(normalized));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                BrandId = p.BrandId,
                BrandName = p.Brand != null ? p.Brand.Name : string.Empty,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable ?? true,
                Stock = p.Inventory != null ? p.Inventory.Stock ?? 0 : 0
            })
            .ToListAsync();
    }
}
