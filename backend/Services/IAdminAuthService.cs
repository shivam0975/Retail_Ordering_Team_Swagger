namespace backend.Services;

public interface IAdminAuthService
{
    Task<ServiceResult<bool>> ValidateAdminAsync(HttpRequest request);
}
