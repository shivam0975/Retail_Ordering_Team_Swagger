using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
    {
        var result = await orderService.CheckoutAsync(request);
        var response = new ApiResponseDto<OrderViewDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var orders = await orderService.GetOrdersByUserAsync(userId);
        return Ok(new ApiResponseDto<List<OrderViewDto>>
        {
            Success = true,
            Message = "Orders fetched successfully.",
            Data = orders
        });
    }
}
