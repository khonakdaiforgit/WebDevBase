namespace MyApp.Application.Abstractions.Subscribers.Dtos
{
    public record SubscriberDto(
        Guid Id,
        string Email,
        DateTime SubscribedAt,
        bool IsActive);
}