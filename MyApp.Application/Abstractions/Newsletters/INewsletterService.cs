using MyApp.Application.Abstractions.Newsletters.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Newsletters
{
    public interface INewsletterService
    {
        Task<Guid> CreateAsync(CreateNewsletterDto dto, Guid callerUserId);
        Task UpdateAsync(UpdateNewsletterDto dto, Guid callerUserId);
        Task DeleteAsync(Guid newsletterId, Guid callerUserId);
        Task SendAsync(Guid newsletterId, Guid callerUserId);
        Task ScheduleAsync(Guid newsletterId, Guid callerUserId);
        Task<NewsletterDto?> GetAsync(Guid newsletterId);
        Task<PagedResult<NewsletterListItemDto>> GetListAsync(Guid? restaurantId = null, int page = 1, int pageSize = 20);
    }
}