namespace MyApp.Application.Abstractions.News.Dtos
{
    public record UpdateNewsDto(Guid Id, string? Title, string? Content, string? ImageUrl, bool? IsPublished);
}