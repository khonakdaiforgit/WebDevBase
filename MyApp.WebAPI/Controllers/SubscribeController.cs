using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Subscribers;
using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Common;
using MyApp.Infrastructure.Common.Exceptions;
using MyApp.WebAPI.Extensions;

namespace MyApp.WebAPI.Controllers;

[Route("api/subscribe")]
[ApiController]
public class SubscribeController : ControllerBase
{
    private readonly IEmailSubscriberService _subscriberService;

    public SubscribeController(IEmailSubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    /// <summary>
    /// Subscribe to restaurant newsletter
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeDto dto)
    {
        try
        {
            await _subscriberService.SubscribeAsync(dto);
            return Ok(new { message = "Confirmation email sent. Please check your inbox." });
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.ToValidationProblemDetails());
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Confirm subscription via email link
    /// </summary>
    [HttpGet("confirm")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { error = "Token is required." });

        try
        {
            await _subscriberService.ConfirmAsync(new ConfirmSubscriptionDto(token));
            return Ok(new { message = "Subscription confirmed successfully!" });
        }
        catch (ValidationException)
        {
            return BadRequest(new { error = "Subscription already confirmed." });
        }
        catch (NotFoundException)
        {
            return NotFound(new { error = "Invalid or expired token." });
        }
    }

    /// <summary>
    /// Unsubscribe via email link
    /// </summary>
    [HttpGet("unsubscribe")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unsubscribe([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { error = "Token is required." });

        try
        {
            await _subscriberService.UnsubscribeAsync(new UnsubscribeDto(token));
            return Ok(new { message = "You have been unsubscribed." });
        }
        catch (ValidationException)
        {
            return BadRequest(new { error = "You are already unsubscribed." });
        }
        catch (NotFoundException)
        {
            return NotFound(new { error = "Invalid token." });
        }
    }

    /// <summary>
    /// Get list of active subscribers (Owner only)
    /// </summary>
    [HttpGet("list/{restaurantId}")]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<SubscriberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<SubscriberDto>>> GetList(
        Guid restaurantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var callerId = this.GetUserId();
        var restaurant = await _subscriberService.GetActiveListAsync(restaurantId, page, pageSize);
        return Ok(restaurant);
    }
}