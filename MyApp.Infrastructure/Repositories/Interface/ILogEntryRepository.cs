using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface ILogEntryRepository : IGenericRepository<LogEntry>
    {
        Task<PagedResult<LogEntry>> GetLogPagedAsync(
            DateTime? from,
            DateTime? to,
            string? level,
            string? project,
            string? userId,
            int page,
            int pageSize,
            CancellationToken ct);

        Task<int> LogCountAsync(DateTime? from, DateTime? to, string? level, CancellationToken ct);
    }
}

