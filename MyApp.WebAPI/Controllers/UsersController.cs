using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Authorization;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Application.Common;
using System.Security.Claims;

[Microsoft.AspNetCore.Authorization.Authorize]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authz;

    public UsersController(IUserService userService, IAuthorizationService authz)
    {
        _userService = userService;
        _authz = authz;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("Invalid user ID in token.");

        var user = await _userService.GetCurrentUserAsync(userId);
        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var callerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (!await _authz.IsOwnerAsync(callerId))
            return Forbid();

        return Ok(await _userService.GetListAsync(page, pageSize));
    }
}