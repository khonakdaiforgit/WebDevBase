using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Subscribers;
using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebAPI.Controllers;

/// <summary>
/// مدیریت مشترکین خبرنامه رستوران (فقط صاحب رستوران دسترسی دارد)
/// </summary>
[Authorize]
[Route("api/subscribers")]
[ApiController]
[Produces("application/json")]
public class SubscribersController : ControllerBase
{
    private readonly IEmailSubscriberService _subscriberService;

    public SubscribersController(IEmailSubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    /// <summary>
    /// دریافت لیست مشترکین فعال (صفحه‌بندی شده) - فقط برای ادمین رستوران
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SubscriberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<SubscriberDto>>> GetList(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 50)
    {
        var result = await _subscriberService.GetListAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// دریافت تمام ایمیل‌های مشترکین فعال (برای ارسال خبرنامه)
    /// </summary>
    [HttpGet("emails")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<string>>> GetActiveEmails()
    {
        var emails = await _subscriberService.GetActiveEmailsAsync();
        return Ok(emails);
    }

    /// <summary>
    /// ثبت‌نام در خبرنامه (از سمت کاربر عمومی - بدون احراز هویت)
    /// </summary>
    [AllowAnonymous]
    [HttpPost("subscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Subscribe([FromBody] string email)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _subscriberService.SubscribeAsync(email.Trim());
        return NoContent(); // یا می‌تونی 200 با پیام موفقیت برگردونی
    }

    /// <summary>
    /// تأیید اشتراک (از طریق لینک ایمیل)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("confirm")]
    [HttpPost("confirm")] // هر دو روش رو ساپورت کن (GET برای لینک، POST برای فرم)
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required.");

        await _subscriberService.ConfirmAsync(token.Trim());
        return NoContent();
    }

    /// <summary>
    /// لغو اشتراک (از طریق لینک یا ایمیل)
    /// </summary>
    [AllowAnonymous]
    [HttpPost("unsubscribe")]
    [HttpGet("unsubscribe")] // برای کلیک روی لینک
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unsubscribe([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required.");

        await _subscriberService.UnsubscribeAsync(token.Trim());
        return NoContent();
    }

    /// <summary>
    /// حذف دستی مشترک (توسط ادمین رستوران) - اختیاری، اگر خواستی
    /// </summary>
    [HttpDelete("{email}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(string email)
    {
        // چون متد UnsubscribeAsync هم با ایمیل کار می‌کنه، می‌تونیم از همون استفاده کنیم
        await _subscriberService.UnsubscribeAsync(email);
        return NoContent();
    }
}