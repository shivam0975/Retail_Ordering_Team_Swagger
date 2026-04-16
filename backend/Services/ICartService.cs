using backend.Dtos;

namespace backend.Services;

public interface ICartService
{
    Task<ServiceResult<CartViewDto>> GetCartByUserAsync(int userId);

    Task<ServiceResult<CartViewDto>> AddItemAsync(AddCartItemRequestDto request);

    Task<ServiceResult<CartViewDto>> UpdateItemAsync(int cartItemId, UpdateCartItemRequestDto request);

    Task<ServiceResult<bool>> RemoveItemAsync(int cartItemId);
}
