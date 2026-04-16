using backend.Dtos;

namespace backend.Services;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);

    Task<ServiceResult<LoginResponseDto>> RegisterAdminAsync(AdminRegisterRequestDto request);

    Task<ServiceResult<LoginResponseDto>> AdminLoginAsync(LoginRequestDto request);

    Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
}
