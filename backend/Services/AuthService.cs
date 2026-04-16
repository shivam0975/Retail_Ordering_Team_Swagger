using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AuthService(RetailOrderingContext context, IConfiguration configuration) : IAuthService
{
    public async Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        return await RegisterUserAsync(request.Name, request.Email, request.Password, "CUSTOMER", "Registration successful.");
    }

    public async Task<ServiceResult<LoginResponseDto>> RegisterAdminAsync(AdminRegisterRequestDto request)
    {
        var expectedSecret = configuration["AdminSettings:RegistrationSecret"];

        if (string.IsNullOrWhiteSpace(expectedSecret))
        {
            return ServiceResult<LoginResponseDto>.Fail("Admin registration is not configured.");
        }

        if (!string.Equals(request.AdminSecret, expectedSecret, StringComparison.Ordinal))
        {
            return ServiceResult<LoginResponseDto>.Fail("Invalid admin registration secret.");
        }

        return await RegisterUserAsync(request.Name, request.Email, request.Password, "ADMIN", "Admin registration successful.");
    }

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        return await LoginInternalAsync(request, adminOnly: false);
    }

    public async Task<ServiceResult<LoginResponseDto>> AdminLoginAsync(LoginRequestDto request)
    {
        return await LoginInternalAsync(request, adminOnly: true);
    }

    private async Task<ServiceResult<LoginResponseDto>> LoginInternalAsync(LoginRequestDto request, bool adminOnly)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<LoginResponseDto>.Fail("Email and password are required.");
        }

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null)
        {
            return ServiceResult<LoginResponseDto>.Fail("Invalid email or password.");
        }

        if (user.PasswordHash != request.Password)
        {
            return ServiceResult<LoginResponseDto>.Fail("Invalid email or password.");
        }

        if (adminOnly && !string.Equals(user.Role, "ADMIN", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<LoginResponseDto>.Fail("Only admin users can login here.");
        }

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = string.IsNullOrWhiteSpace(user.Role) ? "CUSTOMER" : user.Role,
            Token = $"mvp-token-{user.Id}-{DateTime.UtcNow.Ticks}"
        };

        return ServiceResult<LoginResponseDto>.Ok(response, adminOnly ? "Admin login successful." : "Login successful.");
    }

    private async Task<ServiceResult<LoginResponseDto>> RegisterUserAsync(
        string name,
        string email,
        string password,
        string role,
        string successMessage)
    {
        if (string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            return ServiceResult<LoginResponseDto>.Fail("Name, email, and password are required.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var emailExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

        if (emailExists)
        {
            return ServiceResult<LoginResponseDto>.Fail("Email is already registered.");
        }

        var user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = password,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = string.IsNullOrWhiteSpace(user.Role) ? "CUSTOMER" : user.Role,
            Token = $"mvp-token-{user.Id}-{DateTime.UtcNow.Ticks}"
        };

        return ServiceResult<LoginResponseDto>.Ok(response, successMessage);
    }
}
