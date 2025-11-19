using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Subscribers
{
    public interface IEmailSubscriberService
    {
        Task SubscribeAsync(Guid restaurantId, string email);
        Task ConfirmAsync(string token);
        Task UnsubscribeAsync(string emailOrToken);
        Task<List<string>> GetActiveEmailsAsync(Guid restaurantId);
        Task<PagedResult<SubscriberDto>> GetListAsync(Guid restaurantId, int page = 1, int pageSize = 50);
    }
}