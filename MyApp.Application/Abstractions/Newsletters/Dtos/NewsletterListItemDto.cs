namespace MyApp.Application.Abstractions.Newsletters.Dtos
{
    public record NewsletterListItemDto(
        Guid Id,
        string Subject,
        DateTime? SentAt,
        NewsletterStatus Status);
}