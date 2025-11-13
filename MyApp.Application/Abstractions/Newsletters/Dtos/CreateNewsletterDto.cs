namespace MyApp.Application.Abstractions.Newsletters.Dtos
{
    public record CreateNewsletterDto(string Subject, string Content, Guid RestaurantId);
}