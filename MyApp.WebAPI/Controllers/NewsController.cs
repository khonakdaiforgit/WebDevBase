using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.News;
using MyApp.Application.Abstractions.News.Dtos;
using MyApp.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebAPI.Controllers;

[Authorize] 
[Route("api/news")]
[ApiController]
[Produces("application/json")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [AllowAnonymous]
    [HttpGet("public")]
    public async Task<ActionResult<List<NewsListItemDto>>> GetPublicNews()
    {
        var result = await _newsService.GetListAsync(1, 1000); 
        var publishedNews = result.Items
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.PublishDate)
            .ToList();

        return Ok(publishedNews);
    }


    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateNewsDto dto)
    {
        if (dto == null)
            return BadRequest("News data is required.");

        if (string.IsNullOrWhiteSpace(dto.Title) ||
            string.IsNullOrWhiteSpace(dto.Content)
            )
            return BadRequest("Title, Content are required.");

        var newsId = await _newsService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { newsId }, newsId);
    }


    [HttpPut("{newsId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid newsId, [FromBody] UpdateNewsDto dto)
    {
        if (dto == null || dto.Id != newsId)
            return BadRequest("News ID mismatch.");

        await _newsService.UpdateAsync(dto);
        return NoContent();
    }


    [HttpDelete("{newsId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid newsId)
    {
        await _newsService.DeleteAsync(newsId);
        return NoContent();
    }


    [HttpPatch("{newsId}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid newsId)
    {
        await _newsService.PublishAsync(newsId);
        return NoContent();
    }


    [HttpPatch("{newsId}/unpublish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unpublish(Guid newsId)
    {
        await _newsService.UnpublishAsync(newsId);
        return NoContent();
    }


    [HttpGet("{newsId}")]
    [ProducesResponseType(typeof(NewsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NewsDto>> GetById(Guid newsId)
    {
        var news = await _newsService.GetAsync(newsId);
        if (news == null)
            return NotFound();

        return Ok(news);
    }

    [AllowAnonymous]
    [HttpGet("{newsId}/show")]
    public async Task<ActionResult<NewsDto>> GetForPreview(Guid newsId)
    {
        var news = await _newsService.GetAsync(newsId);
        if (news == null)
            return NotFound();

        if (!news.IsPublished)
            return NotFound();

        return Ok(news);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<NewsListItemDto>>> GetAllForAdmin()
    {
        var result = await _newsService.GetListAsync(1, 1000);
        return Ok(result.Items);
    }


    [HttpGet]
    public async Task<ActionResult<PagedResult<NewsListItemDto>>> GetList(
        [FromQuery, Range(1, 100)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20)
    {
        var result = await _newsService.GetListAsync(page, pageSize);
        return Ok(result);
    }
}