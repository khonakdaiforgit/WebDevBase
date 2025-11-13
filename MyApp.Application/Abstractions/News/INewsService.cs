using MyApp.Application.Abstractions.News.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.News
{
    public interface INewsService
    {
        Task<Guid> CreateAsync(CreateNewsDto dto, Guid callerUserId);
        Task UpdateAsync(UpdateNewsDto dto, Guid callerUserId);
        Task DeleteAsync(Guid newsId, Guid callerUserId);
        Task PublishAsync(Guid newsId, Guid callerUserId);
        Task UnpublishAsync(Guid newsId, Guid callerUserId);
        Task<NewsDto?> GetAsync(Guid newsId);
        Task<PagedResult<NewsListItemDto>> GetListAsync(Guid? restaurantId = null, int page = 1, int pageSize = 20);
    }
}