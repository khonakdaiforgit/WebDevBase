using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class MenuCategoryRepository : GenericRepository<MenuCategory>, IMenuCategoryRepository
{
    public MenuCategoryRepository(MongoDbContext context) : base(context) { }

    public async Task<IReadOnlyList<MenuCategory>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default)
    {
        return await _collection
            .Find(c => c.RestaurantId == restaurantId)
            .SortBy(c => c.Order)
            .ToListAsync(ct);
    }
}