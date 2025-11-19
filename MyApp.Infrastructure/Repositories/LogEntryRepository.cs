using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class LogEntryRepository : GenericRepository<LogEntry>, ILogEntryRepository
{
    public LogEntryRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<LogEntry>> GetLogPagedAsync(
        DateTime? from = null,
        DateTime? to = null,
        string? level = null,
        string? project = null,
        string? userId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var filter = Builders<LogEntry>.Filter.Empty;

        if (from.HasValue)
            filter &= Builders<LogEntry>.Filter.Gte(x => x.Timestamp, from.Value);
        if (to.HasValue)
            filter &= Builders<LogEntry>.Filter.Lte(x => x.Timestamp, to.Value);
        if (!string.IsNullOrEmpty(level))
            filter &= Builders<LogEntry>.Filter.Eq(x => x.Level, level);
        if (!string.IsNullOrEmpty(project))
            filter &= Builders<LogEntry>.Filter.Eq(x => x.Project, project);
        if (!string.IsNullOrEmpty(userId))
            filter &= Builders<LogEntry>.Filter.Eq(x => x.UserId, userId);

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection
            .Find(filter)
            .SortByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<LogEntry>(items, (int)total, page, pageSize);
    }
    public async Task<int> LogCountAsync(DateTime? from, DateTime? to, string? level, CancellationToken ct = default)
    {
        var filter = Builders<LogEntry>.Filter.Empty;
        if (from.HasValue) filter &= Builders<LogEntry>.Filter.Gte(x => x.Timestamp, from.Value);
        if (to.HasValue) filter &= Builders<LogEntry>.Filter.Lte(x => x.Timestamp, to.Value);
        if (!string.IsNullOrEmpty(level)) filter &= Builders<LogEntry>.Filter.Eq(x => x.Level, level);

        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }
}