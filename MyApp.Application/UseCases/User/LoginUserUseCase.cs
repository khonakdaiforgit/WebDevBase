using AutoMapper;
using MyApp.Application.DTOs.User;
using MyApp.Domain.Interfaces;
using MyApp.Domain.Interfaces.Services;

namespace MyApp.Application.UseCases.User
{
    public class LoginUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        public LoginUserUseCase(
            IUserRepository userRepository,
            IMapper mapper,
            IJwtService jwtService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserLoginResponseDto> ExecuteAsync(UserLoginDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("The provided email or password is incorrect.");
            if (!user.IsActive)
                throw new UnauthorizedAccessException("The account is currently inactive.");

            var (token, refreshToken, expiresAt) = _jwtService.GenerateTokens(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiresAt.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return new UserLoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
        }
    }
}
