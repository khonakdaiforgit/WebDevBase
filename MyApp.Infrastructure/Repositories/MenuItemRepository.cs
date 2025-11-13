using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
{
    private readonly IMongoCollection<MenuCategory> _categoryCollection;
    public MenuItemRepository(MongoDbContext context) : base(context)
    {
        _categoryCollection = context.GetCollection<MenuCategory>();
    }
    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        var category = await _categoryCollection
                .Find(c => c.Id == categoryId)
                .FirstOrDefaultAsync(ct);

        return category?.Items ?? new List<MenuItem>();
    }

    // اختیاری: آپدیت مستقیم آیتم بدون لود کل دسته
    public async Task UpdateItemInCategoryAsync(Guid categoryId, MenuItem updatedItem, CancellationToken ct = default)
    {
        var filter = Builders<MenuCategory>.Filter.And(
            Builders<MenuCategory>.Filter.Eq(c => c.Id, categoryId),
            Builders<MenuCategory>.Filter.ElemMatch(c => c.Items, i => i.Id == updatedItem.Id)
        );

        var update = Builders<MenuCategory>.Update
            .Set(c => c.Items[-1], updatedItem); // -1 یعنی آخرین المنت (MongoDB Array Update)

        await _categoryCollection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}