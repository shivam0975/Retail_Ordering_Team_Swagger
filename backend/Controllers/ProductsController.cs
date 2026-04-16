using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] int? categoryId)
    {
        var products = await productService.GetProductsAsync(search, categoryId);

        return Ok(new ApiResponseDto<List<ProductListItemDto>>
        {
            Success = true,
            Message = "Products fetched successfully.",
            Data = products
        });
    }
}
