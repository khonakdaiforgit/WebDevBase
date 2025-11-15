using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Logging
{
    public interface ILogService
    {
        Task LogAsync(LogEntryDto logDto, CancellationToken ct = default);
        Task<PagedResult<LogEntryDto>> GetLogsAsync(
            DateTime? from = null,
            DateTime? to = null,
            string? level = null,
            string? project = null,
            string? userId = null,
            int page = 1,
            int pageSize = 50,
            CancellationToken ct = default);

        Task<int> GetCountAsync(
            DateTime? from = null,
            DateTime? to = null,
            string? level = null,
            CancellationToken ct = default);
    }
}