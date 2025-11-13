using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Interface;
using System.Linq.Expressions;

namespace MyApp.Infrastructure.Repositories;

public class LogEntryRepository : ILogEntryRepository
{
    private readonly IMongoCollection<LogEntry> _collection;

    public LogEntryRepository(MongoDbContext context)
    {
        _collection = context.Logs;
    }

    public async Task AddAsync(LogEntry entry, CancellationToken ct = default)
        => await _collection.InsertOneAsync(entry, cancellationToken: ct);

    public async Task<PagedResult<LogEntry>> GetPagedAsync(
        DateTime? from, DateTime? to, string? level, string? project,
        string? userId, Expression<Func<LogEntry, object>>? orderBy,
        bool descending, int page, int pageSize, CancellationToken ct)
    {
        var filter = Builders<LogEntry>.Filter.Empty;

        if (from.HasValue) filter &= Builders<LogEntry>.Filter.Gte(x => x.Timestamp, from.Value);
        if (to.HasValue) filter &= Builders<LogEntry>.Filter.Lte(x => x.Timestamp, to.Value);
        if (!string.IsNullOrEmpty(level)) filter &= Builders<LogEntry>.Filter.Eq(x => x.Level, level);
        if (!string.IsNullOrEmpty(project)) filter &= Builders<LogEntry>.Filter.Eq(x => x.Project, project);
        if (!string.IsNullOrEmpty(userId)) filter &= Builders<LogEntry>.Filter.Eq(x => x.UserId, userId);

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var sort = descending
            ? Builders<LogEntry>.Sort.Descending(orderBy ?? (x => x.Timestamp))
            : Builders<LogEntry>.Sort.Ascending(orderBy ?? (x => x.Timestamp));

        var items = await _collection
            .Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<LogEntry>(items, (int)total, page, pageSize);
    }

    public async Task<int> CountAsync(DateTime? from, DateTime? to, string? level, CancellationToken ct = default)
    {
        var filter = Builders<LogEntry>.Filter.Empty;
        if (from.HasValue) filter &= Builders<LogEntry>.Filter.Gte(x => x.Timestamp, from.Value);
        if (to.HasValue) filter &= Builders<LogEntry>.Filter.Lte(x => x.Timestamp, to.Value);
        if (!string.IsNullOrEmpty(level)) filter &= Builders<LogEntry>.Filter.Eq(x => x.Level, level);

        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }
}