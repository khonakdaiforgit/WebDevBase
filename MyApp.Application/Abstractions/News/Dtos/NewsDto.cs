namespace MyApp.Application.Abstractions.News.Dtos
{
    public record NewsDto(
        Guid Id,
        string Title,
        string Content,
        string ImageUrl,
        DateTime PublishDate,
        bool IsPublished);
}