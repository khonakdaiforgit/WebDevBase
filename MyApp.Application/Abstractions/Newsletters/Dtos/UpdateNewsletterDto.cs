namespace MyApp.Application.Abstractions.Newsletters.Dtos
{
    public record UpdateNewsletterDto(Guid Id, string? Subject, string? Content);
}