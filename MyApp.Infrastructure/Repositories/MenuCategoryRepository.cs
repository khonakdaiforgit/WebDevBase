using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class MenuCategoryRepository : GenericRepository<MenuCategory>, IMenuCategoryRepository
{
    public MenuCategoryRepository(MongoDbContext context) : base(context) { }

    public async Task<int> MaxOrderAsync(CancellationToken ct = default)
    {
        var maxOrder = await _collection
           .Find(Builders<MenuCategory>.Filter.Empty) // همه داکیومنت‌ها
           .SortByDescending(c => c.Order)
           .Project(c => c.Order) // فقط فیلد Order رو بخون (خیلی بهینه)
           .FirstOrDefaultAsync(ct);

        return maxOrder;
    }
}