using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using System.IdentityModel.Tokens.Jwt;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginDto dto)
        => Ok(await _authService.LoginAsync(dto));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResult>> Refresh(RefreshTokenDto dto)
        => Ok(await _authService.RefreshAsync(dto));

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        await _authService.ChangePasswordAsync(userId, dto);
        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        await _authService.LogoutAsync(userId);
        return Ok();
    }
}