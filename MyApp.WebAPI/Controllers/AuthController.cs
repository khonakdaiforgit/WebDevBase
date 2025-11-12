using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.User;
using MyApp.Application.UseCases.User;

namespace MyApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginUserUseCase _loginUseCase;
        private readonly RefreshTokenUseCase _refreshUseCase;
        private readonly IValidator<UserLoginDto> _validator;

        public AuthController(
            LoginUserUseCase loginUseCase,
            RefreshTokenUseCase refreshUseCase,
            IValidator<UserLoginDto> validator)
        {
            _loginUseCase = loginUseCase;
            _refreshUseCase = refreshUseCase;
            _validator = validator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var result = await _loginUseCase.ExecuteAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _refreshUseCase.ExecuteAsync(request.RefreshToken);
            return Ok(result);
        }
    }
}
