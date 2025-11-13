using MyApp.Application.Abstractions.Users.Dtos;

namespace MyApp.Application.Abstractions.Users
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginDto dto);
        Task<AuthResult> RefreshAsync(RefreshTokenDto dto);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task LogoutAsync(Guid userId);
    }
}