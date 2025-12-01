using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(MongoDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Restaurant>> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
    {
        return await _collection
            .Find(r => r.OwnerUserId == ownerUserId)
            .ToListAsync(ct);
    }

    public async Task<Restaurant> GetMain(CancellationToken ct = default)
    {
        return await _collection
                  .Find(r => r.Mian == true)
                  .FirstOrDefaultAsync();
    }

    public async Task<bool> IsOwnerAsync(Guid userId, CancellationToken ct = default)
    {
        var count = await _collection
            .CountDocumentsAsync(r => r.OwnerUserId == userId && r.Mian == true, cancellationToken: ct);

        return count > 0;
    }
}