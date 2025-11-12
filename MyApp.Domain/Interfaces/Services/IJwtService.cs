using MyApp.Domain.Entities;
using System.Security.Claims;


namespace MyApp.Domain.Interfaces.Services
{
    public interface IJwtService
    {
        (string token, string refreshToken, DateTime expiresAt) GenerateTokens(User user);
        ClaimsPrincipal ValidateToken(string token);
        string GenerateRefreshToken();
    }
}
