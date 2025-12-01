using MyApp.Application.Abstractions.News.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.News
{
    public interface INewsService
    {
        Task<Guid> CreateAsync(CreateNewsDto dto);
        Task UpdateAsync(UpdateNewsDto dto);
        Task DeleteAsync(Guid newsId);
        Task PublishAsync(Guid newsId);
        Task UnpublishAsync(Guid newsId);
        Task<NewsDto?> GetAsync(Guid newsId);
        Task<PagedResult<NewsListItemDto>> GetListAsync(int page = 1, int pageSize = 20);
    }
}