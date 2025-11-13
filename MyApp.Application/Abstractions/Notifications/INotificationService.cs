using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Notifications
{
    public interface INotificationService
    {
        Task SendNewsletterAsync(Newsletter newsletter, IReadOnlyList<EmailSubscriber> subscribers);
        Task SendContactConfirmationAsync(string email, string name);
        Task SendPasswordResetAsync(string email, string token);
    }
}