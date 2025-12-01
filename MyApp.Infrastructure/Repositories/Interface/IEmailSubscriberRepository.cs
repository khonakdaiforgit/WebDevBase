using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IEmailSubscriberRepository : IGenericRepository<EmailSubscriber>
    {
        Task<EmailSubscriber?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<EmailSubscriber?> GetByUnsubscribeTokenAsync(string token, CancellationToken ct = default);
        Task<IReadOnlyList<EmailSubscriber>> GetActiveByRestaurantAsync(CancellationToken ct = default);
    }
}
