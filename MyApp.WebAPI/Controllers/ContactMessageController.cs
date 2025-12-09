using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Contacts;
using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebAPI.Controllers;

[Authorize] 
[Route("api/contact-messages")]
[ApiController]
[Produces("application/json")]
public class ContactMessageController : ControllerBase
{
    private readonly IContactMessageService _contactMessageService;

    public ContactMessageController(IContactMessageService contactMessageService)
    {
        _contactMessageService = contactMessageService;
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var count = await _contactMessageService.GetUnreadCountAsync();
        return Ok(count);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContactMessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<ContactMessageDto>>> GetList(
        [FromQuery] bool? onlyUnread = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var result = await _contactMessageService.GetListAsync(onlyUnread, page, pageSize);
        return Ok(result);
    }

    [HttpPatch("{messageId}/mark-as-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid messageId)
    {
        await _contactMessageService.MarkAsReadAsync(messageId);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("public/submit")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> SubmitPublic(
        [FromBody] SubmitContactMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            ModelState.AddModelError(nameof(request.Name), "Name is required.");

        if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
            ModelState.AddModelError(nameof(request.Email), "A valid email address is required.");

        if (string.IsNullOrWhiteSpace(request.Message))
            ModelState.AddModelError(nameof(request.Message), "Message is required.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var messageId = await _contactMessageService.SubmitAsync(
            request.Name.Trim(),
            request.Email.Trim(),
            request.Message.Trim());

        return CreatedAtAction(nameof(SubmitPublic), new { id = messageId }, messageId);
    }
}

public record SubmitContactMessageRequest(
    string Name,
    string Email,
    string Message);