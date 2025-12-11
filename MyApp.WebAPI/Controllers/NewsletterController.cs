using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Newsletters;
using MyApp.Application.Abstractions.Newsletters.Dtos;
using MyApp.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebAPI.Controllers;

[Authorize] 
[Route("api/newsletters")]
[ApiController]
[Produces("application/json")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _newsletterService;

    public NewsletterController(INewsletterService newsletterService)
    {
        _newsletterService = newsletterService;
    }

    /// <summary>
    /// ایجاد کمپین خبرنامه جدید (به صورت Draft)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateNewsletterDto dto)
    {
        if (dto == null)
            return BadRequest("Newsletter data is required.");

        if (string.IsNullOrWhiteSpace(dto.Subject) || string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("Subject and Content are required.");

        var newsletterId = await _newsletterService.CreateAsync(dto.Subject, dto.Content);
        return CreatedAtAction(nameof(GetById), new { newsletterId }, newsletterId);
    }

    /// <summary>
    /// ویرایش خبرنامه (فقط اگر هنوز ارسال نشده باشد)
    /// </summary>
    [HttpPut("{newsletterId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid newsletterId, [FromBody] UpdateNewsletterDto dto)
    {
        if (dto == null || dto.Id != newsletterId)
            return BadRequest("Newsletter ID mismatch.");

        await _newsletterService.UpdateAsync(dto.Id, dto.Subject, dto.Content);
        return NoContent();
    }

    /// <summary>
    /// حذف خبرنامه (فقط اگر Draft یا Scheduled باشد - در سرویس چک می‌شود)
    /// </summary>
    [HttpDelete("{newsletterId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid newsletterId)
    {
        await _newsletterService.DeleteAsync(newsletterId);
        return NoContent();
    }

    /// <summary>
    /// ارسال فوری خبرنامه به تمام مشترکین فعال
    /// </summary>
    [HttpPost("{newsletterId}/send-now")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendNow(Guid newsletterId)
    {
        await _newsletterService.SendNowAsync(newsletterId);
        return NoContent();
    }

    /// <summary>
    /// دریافت جزئیات یک خبرنامه (برای ویرایش یا مشاهده وضعیت)
    /// </summary>
    [HttpGet("{newsletterId}")]
    [ProducesResponseType(typeof(NewsletterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NewsletterDto>> GetById(Guid newsletterId)
    {
        var newsletter = await _newsletterService.GetAsync(newsletterId);
        if (newsletter == null)
            return NotFound();

        return Ok(newsletter);
    }

    /// <summary>
    /// لیست صفحه‌بندی شده خبرنامه‌ها برای پنل مدیریت
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<NewsletterListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<NewsletterListItemDto>>> GetList(
        [FromQuery, Range(1, 1000)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20)
    {
        var result = await _newsletterService.GetListAsync(page, pageSize);
        return Ok(result);
    }
}

