namespace MyApp.Application.Abstractions.Contacts.Dtos
{
    public record ContactMessageDto(
        Guid Id,
        string Name,
        string Email,
        string Message,
        DateTime SentAt,
        bool IsRead);
}