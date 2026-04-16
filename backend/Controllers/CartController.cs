using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetCart(int userId)
    {
        var result = await cartService.GetCartByUserAsync(userId);
        var response = new ApiResponseDto<CartViewDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequestDto request)
    {
        var result = await cartService.AddItemAsync(request);
        var response = new ApiResponseDto<CartViewDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemRequestDto request)
    {
        var result = await cartService.UpdateItemAsync(cartItemId, request);
        var response = new ApiResponseDto<CartViewDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var result = await cartService.RemoveItemAsync(cartItemId);
        var response = new ApiResponseDto<bool>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : NotFound(response);
    }
}
