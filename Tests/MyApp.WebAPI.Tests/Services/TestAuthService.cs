// Tests/Services/TestAuthService.cs
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common.Exceptions;

public class TestAuthService : IAuthService
{
    private static readonly UserDto TestUser = new(
        Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Email: "admin@example.com",
        Role: UserRole.Admin,
        IsActive: true,
        CreatedAt: DateTime.UtcNow.AddDays(-30)
    );

    public Task<AuthResult> LoginAsync(LoginDto dto)
    {
        if (dto.Email == "admin@example.com" && dto.Password == "Password123!")
        {
            return Task.FromResult(new AuthResult(
                AccessToken: "valid-jwt-token",
                RefreshToken: "refresh-token-123",
                ExpiresAt: DateTime.UtcNow.AddHours(1),
                User: TestUser
            ));
        }
        throw new UnauthorizedAccessException("Invalid credentials");
    }

    public Task<AuthResult> RefreshAsync(RefreshTokenDto dto)
    {
        if (dto.RefreshToken == "refresh-token-123")
        {
            return Task.FromResult(new AuthResult(
                AccessToken: "new-jwt-token",
                RefreshToken: "new-refresh-456",
                ExpiresAt: DateTime.UtcNow.AddHours(1),
                User: TestUser
            ));
        }
        throw new UnauthorizedAccessException("Invalid refresh token");
    }

    public Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword.Length >= 6)
            return Task.CompletedTask;
        throw new ValidationException("Password mismatch or too short");
    }

    public Task LogoutAsync(Guid userId) => Task.CompletedTask;
}