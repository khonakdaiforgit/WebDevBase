using MyApp.Application.Common;
using MyApp.Domain.Entities;
using System.Linq.Expressions;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface ILogEntryRepository
    {
        Task AddAsync(LogEntry entry, CancellationToken ct = default);

        Task<PagedResult<LogEntry>> GetPagedAsync(
            DateTime? from = null,
            DateTime? to = null,
            string? level = null,
            string? project = null,
            string? userId = null,
            Expression<Func<LogEntry, object>>? orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = 50,
            CancellationToken ct = default);

        Task<int> CountAsync(
            DateTime? from = null,
            DateTime? to = null,
            string? level = null,
            CancellationToken ct = default);
    }
}
