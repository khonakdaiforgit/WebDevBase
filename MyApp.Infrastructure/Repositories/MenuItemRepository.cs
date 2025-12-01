using MongoDB.Bson;
using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(MongoDbContext context) : base(context) { }

        public async Task<IReadOnlyList<MenuItem>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
        {
            return await _collection
                        .Find(item => item.CategoryId == categoryId && item.IsAvailable)
                        .SortBy(item => item.Name)
                        .ToListAsync(ct);
        }

        public async Task<int> MaxOrderAsync(CancellationToken ct = default)
        {
            var maxOrder = await _collection
               .Find(Builders<MenuItem>.Filter.Empty) // همه داکیومنت‌ها
               .SortByDescending(c => c.Order)
               .Project(c => c.Order) // فقط فیلد Order رو بخون (خیلی بهینه)
               .FirstOrDefaultAsync(ct);

            return maxOrder;
        }

        public async Task ToggleAvailabilityAsync(Guid itemId, CancellationToken ct = default)
        {
            var filter = Builders<MenuItem>.Filter.Eq(x => x.Id, itemId);

            var update = Builders<MenuItem>.Update
                .BitwiseXor(x => x.IsAvailable, true);

            var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException($"Menu item with ID {itemId} was not found.");
        }
    }
}