using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyApp.Application.Common;
using MyApp.Domain.Interfaces.Common;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Interface.Common;
using System.Linq.Expressions;

namespace MyApp.Infrastructure.Repositories.Common;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IHasId<Guid>
{
    protected readonly IMongoCollection<T> _collection;

    public GenericRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken ct = default)
    {
        var query = filter == null ? _collection.AsQueryable() : _collection.AsQueryable().Where(filter);
        return await query.ToListAsync(ct);
    }

    public async Task<PagedResult<T>> GetPagedAsync(
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = filter == null ? _collection.AsQueryable() : _collection.AsQueryable().Where(filter);

        if (orderBy != null)
        {
            query = orderByDescending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>(items, total, page, pageSize);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        => await _collection.InsertManyAsync(entities, cancellationToken: ct);

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter, ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        => await _collection.Find(filter).AnyAsync(ct);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        => filter == null
            ? (int)await _collection.CountDocumentsAsync(Builders<T>.Filter.Empty, cancellationToken: ct)
            : (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
}