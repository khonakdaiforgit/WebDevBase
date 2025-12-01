// src/Infrastructure/Repositories/NewsRepository.cs
using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class NewsRepository : GenericRepository<News>, INewsRepository
{
    public NewsRepository(MongoDbContext context) : base(context) { }

    public async Task<IReadOnlyList<News>> GetPublishedByRestaurantAsync(CancellationToken ct = default)
    {
        return await _collection
            .Find(Builders<News>.Filter.Empty) // همه داکیومنت‌ها
            .SortByDescending(n => n.PublishDate)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<News>> GetPagedByRestaurantAsync(
        bool? onlyPublished = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = Builders<News>.Filter.Empty;
        if (onlyPublished == true)
            filter = Builders<News>.Filter.Eq(n => n.IsPublished, true);

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        var items = await _collection
            .Find(filter)
            .SortByDescending(n => n.PublishDate)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<News>(items, (int)total, page, pageSize);
    }
}