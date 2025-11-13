namespace MyApp.Application.Abstractions.News.Dtos
{
    public record CreateNewsDto(string Title, string Content, string ImageUrl, Guid RestaurantId);
}