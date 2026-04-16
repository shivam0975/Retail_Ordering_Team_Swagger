using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("admin-login")]
    public async Task<IActionResult> AdminLogin([FromBody] LoginRequestDto request)
    {
        var result = await authService.AdminLoginAsync(request);
        var response = new ApiResponseDto<LoginResponseDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterRequestDto request)
    {
        var result = await authService.RegisterAdminAsync(request);
        var response = new ApiResponseDto<LoginResponseDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await authService.RegisterAsync(request);
        var response = new ApiResponseDto<LoginResponseDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await authService.LoginAsync(request);
        var response = new ApiResponseDto<LoginResponseDto>
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

        return result.Success ? Ok(response) : BadRequest(response);
    }
}
