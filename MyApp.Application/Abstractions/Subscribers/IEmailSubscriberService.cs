using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Subscribers
{
    public interface IEmailSubscriberService
    {
        Task SubscribeAsync(SubscribeDto dto);
        Task ConfirmAsync(ConfirmSubscriptionDto dto);
        Task UnsubscribeAsync(UnsubscribeDto dto);
        Task<PagedResult<SubscriberDto>> GetActiveListAsync(Guid restaurantId, int page = 1, int pageSize = 50);
    }
}