using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface IEmailSubscriberRepository : IRepository<EmailSubscriber>
    {
        Task<EmailSubscriber> GetByEmailAsync(string email);
        Task<EmailSubscriber> GetByTokenAsync(string token);
        Task<List<EmailSubscriber>> GetActiveSubscribersAsync();
    }
}
