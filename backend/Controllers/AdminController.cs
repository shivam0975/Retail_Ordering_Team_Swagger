using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminAuthService adminAuthService, IAdminService adminService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var summary = await adminService.GetDashboardSummaryAsync();

        return Ok(new ApiResponseDto<DashboardSummaryDto>
        {
            Success = true,
            Message = "Dashboard loaded successfully.",
            Data = summary
        });
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var products = await adminService.GetProductsAsync();
        return Ok(new ApiResponseDto<List<ProductListItemDto>>
        {
            Success = true,
            Message = "Products loaded successfully.",
            Data = products
        });
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] AdminProductUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.CreateProductAsync(request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpPut("products/{productId:int}")]
    public async Task<IActionResult> UpdateProduct(int productId, [FromBody] AdminProductUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.UpdateProductAsync(productId, request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpDelete("products/{productId:int}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.DeleteProductAsync(productId);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var categories = await adminService.GetCategoriesAsync();
        return Ok(new ApiResponseDto<List<AdminCategoryDto>>
        {
            Success = true,
            Message = "Categories loaded successfully.",
            Data = categories
        });
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] AdminCategoryUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.CreateCategoryAsync(request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpPut("categories/{categoryId:int}")]
    public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] AdminCategoryUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.UpdateCategoryAsync(categoryId, request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpDelete("categories/{categoryId:int}")]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.DeleteCategoryAsync(categoryId);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var brands = await adminService.GetBrandsAsync();
        return Ok(new ApiResponseDto<List<AdminBrandDto>>
        {
            Success = true,
            Message = "Brands loaded successfully.",
            Data = brands
        });
    }

    [HttpPost("brands")]
    public async Task<IActionResult> CreateBrand([FromBody] AdminBrandUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.CreateBrandAsync(request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpPut("brands/{brandId:int}")]
    public async Task<IActionResult> UpdateBrand(int brandId, [FromBody] AdminBrandUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.UpdateBrandAsync(brandId, request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpDelete("brands/{brandId:int}")]
    public async Task<IActionResult> DeleteBrand(int brandId)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.DeleteBrandAsync(brandId);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpGet("coupons")]
    public async Task<IActionResult> GetCoupons()
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var coupons = await adminService.GetCouponsAsync();
        return Ok(new ApiResponseDto<List<AdminCouponDto>>
        {
            Success = true,
            Message = "Coupons loaded successfully.",
            Data = coupons
        });
    }

    [HttpPost("coupons")]
    public async Task<IActionResult> CreateCoupon([FromBody] AdminCouponUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.CreateCouponAsync(request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpPut("coupons/{couponId:int}")]
    public async Task<IActionResult> UpdateCoupon(int couponId, [FromBody] AdminCouponUpsertDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.UpdateCouponAsync(couponId, request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpDelete("coupons/{couponId:int}")]
    public async Task<IActionResult> DeleteCoupon(int couponId)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.DeleteCouponAsync(couponId);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var orders = await adminService.GetOrdersAsync();
        return Ok(new ApiResponseDto<List<AdminOrderDto>>
        {
            Success = true,
            Message = "Orders loaded successfully.",
            Data = orders
        });
    }

    [HttpPut("orders/{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] AdminOrderStatusUpdateDto request)
    {
        var authResult = await EnsureAdminAsync();
        if (authResult is not null)
        {
            return authResult;
        }

        var result = await adminService.UpdateOrderStatusAsync(orderId, request);
        return result.Success ? Ok(ToResponse(result)) : BadRequest(ToResponse(result));
    }

    private async Task<IActionResult?> EnsureAdminAsync()
    {
        var authResult = await adminAuthService.ValidateAdminAsync(Request);
        if (authResult.Success)
        {
            return null;
        }

        return Unauthorized(new ApiResponseDto<object>
        {
            Success = false,
            Message = authResult.Message,
            Data = null
        });
    }

    private static ApiResponseDto<T> ToResponse<T>(ServiceResult<T> result)
    {
        return new ApiResponseDto<T>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };
    }
}
