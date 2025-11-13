using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Users.Dtos
{
    public record AuthResult(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt,
        UserDto User);
}