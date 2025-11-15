using MyApp.Application.Common;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface ILogEntryRepository
    {
        Task AddAsync(LogEntry entry, CancellationToken ct = default);

        Task<PagedResult<LogEntry>> GetPagedAsync(
            DateTime? from,
            DateTime? to,
            string? level,
            string? project,
            string? userId,
            int page,
            int pageSize,
            CancellationToken ct);

        Task<int> CountAsync(DateTime? from, DateTime? to, string? level, CancellationToken ct);
    }
}

