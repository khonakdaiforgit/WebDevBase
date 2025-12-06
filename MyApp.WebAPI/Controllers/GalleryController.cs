using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Galleries;
using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebAPI.Controllers;

[Authorize]
[Route("api/gallery")]
[ApiController]
[Produces("application/json")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    // اضافه کن بالای کلاس یا داخلش
    [AllowAnonymous] // مهم: این endpoint عمومی است
    [HttpGet("public")]
    public async Task<ActionResult<List<GalleryItemDto>>> GetPublicGallery()
    {
        // فقط تصاویر visible و مرتب شده بر اساس تاریخ
        var result = await _galleryService.GetForRestaurantAsync(1, 1000);
        var visibleItems = result.Items
            .Where(x => x.IsVisible)
            .OrderByDescending(x => x.UploadDate)
            .ToList();

        return Ok(visibleItems);
    }

    /// <summary>
    /// آپلود تصویر جدید در گالری
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Upload([FromBody] UploadGalleryItemDto request)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl))
            return BadRequest("ImageUrl is required.");

        var dto = new UploadGalleryItemDto(
            ImageUrl: request.ImageUrl.Trim(),
            Caption: request.Caption?.Trim() ?? string.Empty
        );

        var itemId = await _galleryService.UploadAsync(dto);
        return CreatedAtAction(nameof(GetItem), new { itemId }, itemId);
    }

    /// <summary>
    /// ویرایش کپشن یا وضعیت نمایش تصویر
    /// </summary>
    [HttpPut("{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid itemId, [FromBody] UpdateGalleryItemDto dto)
    {

        await _galleryService.UpdateAsync(dto);
        return NoContent();
    }

    /// <summary>
    /// حذف دائمی تصویر از گالری
    /// </summary>
    [HttpDelete("{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid itemId)
    {
        await _galleryService.DeleteAsync(itemId);
        return NoContent();
    }

    /// <summary>
    /// مخفی کردن تصویر (از نمایش در سایت)
    /// </summary>
    [HttpPatch("{itemId}/hide")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Hide(Guid itemId)
    {
        await _galleryService.HideAsync(itemId);
        return NoContent();
    }

    /// <summary>
    /// نمایش دوباره تصویر در سایت
    /// </summary>
    [HttpPatch("{itemId}/show")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Show(Guid itemId)
    {
        await _galleryService.ShowAsync(itemId);
        return NoContent();
    }

    /// <summary>
    /// دریافت یک تصویر خاص (برای ویرایش در داشبورد)
    /// </summary>
    [HttpGet("{itemId}")]
    [ProducesResponseType(typeof(GalleryItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GalleryItemDto>> GetItem(Guid itemId)
    {
        var result = await _galleryService.GetForRestaurantAsync(1, 1000);
        var item = result.Items.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    // 1. برای داشبورد ادمین (همه رو یکجا بده)
    [HttpGet("all")]
    public async Task<ActionResult<List<GalleryItemDto>>> GetAllForAdmin()
    {
        var result = await _galleryService.GetForRestaurantAsync(1, 1000);
        return Ok(result.Items);
    }

    // 2. برای بقیه موارد (صفحه‌بندی شده بمونه)
    [HttpGet]
    public async Task<ActionResult<PagedResult<GalleryItemDto>>> GetList(
        [FromQuery, Range(1, 100)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20)
    {
        var result = await _galleryService.GetForRestaurantAsync(page, pageSize);
        return Ok(result);
    }
}