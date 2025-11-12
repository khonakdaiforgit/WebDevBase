using AutoMapper;
using Microsoft.AspNetCore.Http;
using MyApp.Application.DTOs.User;
using MyApp.Domain.Interfaces;
using System.Security.Claims;


namespace MyApp.Application.UseCases.User
{
    public class GetCurrentUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public GetCurrentUserUseCase(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<UserDto> ExecuteAsync()
        {
            var userIdClaim = _httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException();

            var user = await _userRepository.GetAsync(userId);
            return _mapper.Map<UserDto>(user);
        }
    }
}
