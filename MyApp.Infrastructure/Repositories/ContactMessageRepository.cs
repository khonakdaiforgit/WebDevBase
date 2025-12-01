using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;
using System.Linq.Expressions;

namespace MyApp.Infrastructure.Repositories;

public class ContactMessageRepository : GenericRepository<ContactMessage>, IContactMessageRepository
{
    public ContactMessageRepository(MongoDbContext context) : base(context) { }

    public async Task<PagedResult<ContactMessage>> GetPagedAsync(
        bool? onlyUnread = null,
        Expression<Func<ContactMessage, object>>? orderBy = null,
        bool descending = true,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = Builders<ContactMessage>.Filter.Empty;
        if (onlyUnread == true)
            filter &= Builders<ContactMessage>.Filter.Eq(m => m.IsRead, false);

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var sort = descending
            ? Builders<ContactMessage>.Sort.Descending(orderBy ?? (m => m.SentAt))
            : Builders<ContactMessage>.Sort.Ascending(orderBy ?? (m => m.SentAt));

        var items = await _collection
            .Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<ContactMessage>(items, (int)total, page, pageSize);
    }
}