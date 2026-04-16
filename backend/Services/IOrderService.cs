using backend.Dtos;

namespace backend.Services;

public interface IOrderService
{
    Task<ServiceResult<OrderViewDto>> CheckoutAsync(CheckoutRequestDto request);

    Task<List<OrderViewDto>> GetOrdersByUserAsync(int userId);
}
