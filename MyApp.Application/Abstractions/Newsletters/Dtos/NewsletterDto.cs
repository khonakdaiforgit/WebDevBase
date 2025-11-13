namespace MyApp.Application.Abstractions.Newsletters.Dtos
{
    public record NewsletterDto(
        Guid Id,
        string Subject,
        string Content,
        DateTime? SentAt,
        Guid SentByUserId,
        NewsletterStatus Status);
}