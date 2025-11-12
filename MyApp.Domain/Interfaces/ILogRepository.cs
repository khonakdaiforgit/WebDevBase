using MongoDB.Driver;
using MyApp.Domain.Dtos;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface ILogRepository : IRepository<LogEntry>
    {
        Task InsertLogAsync(LogEntry logEntry);
        Task<IEnumerable<LogEntry>> GetAllLogsAsync();
        Task<IEnumerable<LogEntry>> GetLogsByUserAsync(string userId);
        Task<IEnumerable<LogEntry>> GetLogsByLevelAsync(string level);
        IEnumerable<LogEntry> GetFilteredLogsAsync(FilterDefinition<LogEntry> filter);
        Task<PagedLogResult> GetFilteredLogsWithPagingAsync(FilterDefinition<LogEntry> filter, int pageNumber, int pageSize);
    }
}
