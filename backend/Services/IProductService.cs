using backend.Dtos;

namespace backend.Services;

public interface IProductService
{
    Task<List<ProductListItemDto>> GetProductsAsync(string? search, int? categoryId);
}
