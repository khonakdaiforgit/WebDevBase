using MongoDB.Driver;
using MyApp.Application.Common;
using MyApp.Domain.Interfaces.Common;
using MyApp.Infrastructure.Repositories.Interface.Common;
using System.Linq.Expressions;

namespace MyApp.WebAPI.Tests.InMemory
{
    public class InMemoryGenericRepository<T> : IGenericRepository<T> where T : class, IHasId<Guid>
    {
        private readonly List<T> _items = new();
        private readonly object _lock = new();

        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(x => x.Id == id);
                return Task.FromResult(item);
            }
        }

        public Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            CancellationToken ct = default)
        {
            lock (_lock)
            {
                var query = _items.AsQueryable();
                if (filter != null)
                    query = query.Where(filter);

                return Task.FromResult(query.ToList() as IReadOnlyList<T>);
            }
        }

        public Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool orderByDescending = false,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default)
        {
            lock (_lock)
            {
                var query = _items.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                var total = query.Count();

                if (orderBy != null)
                {
                    query = orderByDescending
                        ? query.OrderByDescending(orderBy)
                        : query.OrderBy(orderBy);
                }

                var items = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Task.FromResult(new PagedResult<T>(items, total, page, pageSize));
            }
        }

        public Task AddAsync(T entity, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            lock (_lock)
            {
                if (entity.Id == Guid.Empty)
                    entity.GetType().GetProperty("Id")?.SetValue(entity, Guid.NewGuid());

                if (_items.Any(x => x.Id == entity.Id))
                    throw new InvalidOperationException($"Entity with Id {entity.Id} already exists.");

                _items.Add(entity);
            }

            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            lock (_lock)
            {
                foreach (var entity in entities)
                {
                    if (entity.Id == Guid.Empty)
                        entity.GetType().GetProperty("Id")?.SetValue(entity, Guid.NewGuid());

                    if (_items.Any(x => x.Id == entity.Id))
                        throw new InvalidOperationException($"Entity with Id {entity.Id} already exists.");
                }

                _items.AddRange(entities);
            }

            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            lock (_lock)
            {
                var existing = _items.FirstOrDefault(x => x.Id == entity.Id);
                if (existing == null)
                    throw new KeyNotFoundException($"Entity with Id {entity.Id} not found.");

                var index = _items.IndexOf(existing);
                _items[index] = entity;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            lock (_lock)
            {
                var entity = _items.FirstOrDefault(x => x.Id == id);
                if (entity != null)
                    _items.Remove(entity);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            lock (_lock)
            {
                return Task.FromResult(_items.AsQueryable().Any(filter));
            }
        }

        public Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            lock (_lock)
            {
                return Task.FromResult(filter == null ? _items.Count : _items.AsQueryable().Count(filter));
            }
        }

        // متد کمکی برای تست: پاک کردن داده‌ها
        public void Clear()
        {
            lock (_lock)
            {
                _items.Clear();
            }
        }

        // متد کمکی برای تزریق داده اولیه
        public void Seed(IEnumerable<T> seedData)
        {
            lock (_lock)
            {
                Clear();
                foreach (var item in seedData)
                {
                    if (item.Id == Guid.Empty)
                        item.GetType().GetProperty("Id")?.SetValue(item, Guid.NewGuid());
                }
                _items.AddRange(seedData);
            }
        }
    }
}
