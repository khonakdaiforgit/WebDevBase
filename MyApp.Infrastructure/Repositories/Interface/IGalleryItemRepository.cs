using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IGalleryItemRepository : IGenericRepository<GalleryItem>
    {
        Task<PagedResult<GalleryItem>> GetVisibleByRestaurantAsync(
            Guid restaurantId,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);
    }
}
