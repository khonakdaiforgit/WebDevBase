// src/Infrastructure/Repositories/GalleryItemRepository.cs
using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class GalleryItemRepository : GenericRepository<GalleryItem>, IGalleryItemRepository
{
    public GalleryItemRepository(MongoDbContext context) : base(context) { }

    public async Task<PagedResult<GalleryItem>> GetVisibleByRestaurantAsync(
        Guid restaurantId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = Builders<GalleryItem>.Filter.Eq(x => x.RestaurantId, restaurantId) &
                     Builders<GalleryItem>.Filter.Eq(x => x.IsVisible, true);

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        var items = await _collection
            .Find(filter)
            .SortByDescending(x => x.UploadDate)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<GalleryItem>(items, (int)total, page, pageSize);
    }
}