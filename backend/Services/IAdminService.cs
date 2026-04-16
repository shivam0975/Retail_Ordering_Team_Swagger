using backend.Dtos;

namespace backend.Services;

public interface IAdminService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();

    Task<List<ProductListItemDto>> GetProductsAsync();

    Task<ServiceResult<ProductListItemDto>> CreateProductAsync(AdminProductUpsertDto request);

    Task<ServiceResult<ProductListItemDto>> UpdateProductAsync(int productId, AdminProductUpsertDto request);

    Task<ServiceResult<bool>> DeleteProductAsync(int productId);

    Task<List<AdminCategoryDto>> GetCategoriesAsync();

    Task<ServiceResult<AdminCategoryDto>> CreateCategoryAsync(AdminCategoryUpsertDto request);

    Task<ServiceResult<AdminCategoryDto>> UpdateCategoryAsync(int categoryId, AdminCategoryUpsertDto request);

    Task<ServiceResult<bool>> DeleteCategoryAsync(int categoryId);

    Task<List<AdminBrandDto>> GetBrandsAsync();

    Task<ServiceResult<AdminBrandDto>> CreateBrandAsync(AdminBrandUpsertDto request);

    Task<ServiceResult<AdminBrandDto>> UpdateBrandAsync(int brandId, AdminBrandUpsertDto request);

    Task<ServiceResult<bool>> DeleteBrandAsync(int brandId);

    Task<List<AdminCouponDto>> GetCouponsAsync();

    Task<ServiceResult<AdminCouponDto>> CreateCouponAsync(AdminCouponUpsertDto request);

    Task<ServiceResult<AdminCouponDto>> UpdateCouponAsync(int couponId, AdminCouponUpsertDto request);

    Task<ServiceResult<bool>> DeleteCouponAsync(int couponId);

    Task<List<AdminOrderDto>> GetOrdersAsync();

    Task<ServiceResult<AdminOrderDto>> UpdateOrderStatusAsync(int orderId, AdminOrderStatusUpdateDto request);
}
