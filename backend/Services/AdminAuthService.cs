using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AdminAuthService(RetailOrderingContext context) : IAdminAuthService
{
    public async Task<ServiceResult<bool>> ValidateAdminAsync(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-User-Id", out var userIdRaw)
            || !int.TryParse(userIdRaw.ToString(), out var userId)
            || userId <= 0)
        {
            return ServiceResult<bool>.Fail("Admin authorization requires X-User-Id header.");
        }

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return ServiceResult<bool>.Fail("Admin user not found.");
        }

        if (!string.Equals(user.Role, "ADMIN", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<bool>.Fail("Only admin users can access this endpoint.");
        }

        return ServiceResult<bool>.Ok(true, "Admin authorized.");
    }
}
