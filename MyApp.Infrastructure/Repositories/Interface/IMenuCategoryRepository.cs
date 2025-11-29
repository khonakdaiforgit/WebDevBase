using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IMenuCategoryRepository : IGenericRepository<MenuCategory>
    {
        Task<IReadOnlyList<MenuCategory>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
    }
}
