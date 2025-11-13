using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface INewsRepository : IGenericRepository<News>
    {
        Task<IReadOnlyList<News>> GetPublishedByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
        Task<PagedResult<News>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            bool? onlyPublished = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);
    }
}
