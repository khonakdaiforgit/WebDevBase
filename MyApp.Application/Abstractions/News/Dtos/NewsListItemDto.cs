namespace MyApp.Application.Abstractions.News.Dtos
{
    public record NewsListItemDto(
        Guid Id,
        string Title,
        string ImageUrl,
        DateTime PublishDate,
        bool IsPublished);
}