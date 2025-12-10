namespace MyApp.Application.Abstractions.News.Dtos
{
    public record NewsListItemDto(
        Guid Id,
        string Title,
        string ImageUrl,
        string Content,
        DateTime PublishDate,
        bool IsPublished);
}