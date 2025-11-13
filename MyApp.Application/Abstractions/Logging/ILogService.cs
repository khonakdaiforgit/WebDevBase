using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Common;
using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Logging
{
    public interface ILogService
    {
        Task LogAsync(LogEntry entry); // استفاده داخلی
        Task<PagedResult<LogEntryDto>> GetLogsAsync(
            DateTime? from = null,
            DateTime? to = null,
            string? level = null,
            string? project = null,
            int page = 1,
            int pageSize = 50);
    }
}