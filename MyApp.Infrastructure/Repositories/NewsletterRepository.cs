using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class NewsletterRepository : GenericRepository<Newsletter>, INewsletterRepository
{
    public NewsletterRepository(MongoDbContext context) : base(context) { }

    public async Task<PagedResult<Newsletter>> GetPagedByRestaurantAsync(
        NewsletterStatus? status = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = Builders<Newsletter>.Filter.Empty;
        if (status.HasValue)
            filter &= Builders<Newsletter>.Filter.Eq(n => n.Status, status.Value);

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        var items = await _collection
           .Find(filter)
           .SortByDescending(n => n.SentAt ?? DateTime.MinValue)
           .ThenByDescending(n => n.Id)
           .Skip((page - 1) * pageSize)
           .Limit(pageSize)
           .ToListAsync(ct);

        return new PagedResult<Newsletter>(items, (int)total, page, pageSize);
    }
}