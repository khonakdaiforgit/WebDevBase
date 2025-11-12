using MyApp.Application.DTOs.User;
using MyApp.Domain.Interfaces;
using MyApp.Domain.Interfaces.Services;

namespace MyApp.Application.UseCases.User
{
    public class RefreshTokenUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenUseCase(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<UserLoginResponseDto> ExecuteAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("The token has expired.");

            var (newToken, newRefreshToken, expiresAt) = _jwtService.GenerateTokens(user);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = expiresAt.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return new UserLoginResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt
            };
        }
    }
}
