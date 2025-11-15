using MyApp.Application.Abstractions.Logging;
using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;

namespace MyApp.Infrastructure.Services.Logging;

public class LogService : ILogService
{
    private readonly IUnitOfWork _unitOfWork;

    public LogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(LogEntryDto logDto, CancellationToken ct = default)
    {
        var entity = MapToEntity(logDto);
        await _unitOfWork.Logs.AddAsync(entity, ct);
    }

    public async Task<PagedResult<LogEntryDto>> GetLogsAsync(
        DateTime? from = null,
        DateTime? to = null,
        string? level = null,
        string? project = null,
        string? userId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var paged = await _unitOfWork.Logs.GetPagedAsync(
            from: from,
            to: to,
            level: level,
            project: project,
            userId: userId,
            page: page,
            pageSize: pageSize,
            ct: ct);

        var dtos = paged.Items.Select(MapToDto).ToList();
        return new PagedResult<LogEntryDto>(dtos, paged.TotalCount, page, pageSize);
    }

    public async Task<int> GetCountAsync(
        DateTime? from = null,
        DateTime? to = null,
        string? level = null,
        CancellationToken ct = default)
    {
        return await _unitOfWork.Logs.CountAsync(from, to, level, ct);
    }

    private static LogEntryDto MapToDto(LogEntry log) => new(
      Id: log.Id,
      Timestamp: log.Timestamp,
      Level: log.Level,
      Message: log.Message ?? string.Empty,
      ExceptionMessage: log.Exception?.Split('\n').FirstOrDefault(),
      StackTrace: log.Exception,
      UserId: log.UserId ?? "Anonymous",
      HashedIp: log.HashedIp,
      Method: log.Method,
      Path: log.Route,
      StatusCode: log.StatusCode ?? 0,
      Project: log.Project,
      DurationMs: log.DurationMs
    );

    private static LogEntry MapToEntity(LogEntryDto dto) => new()
    {
        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
        Timestamp = dto.Timestamp == default ? DateTime.UtcNow : dto.Timestamp,
        Level = dto.Level,
        Message = dto.Message,
        Exception = CombineException(dto.ExceptionMessage, dto.StackTrace),
        UserId = dto.UserId == "Anonymous" ? null : dto.UserId,
        HashedIp = dto.HashedIp,
        Route = dto.Path,
        Method = dto.Method,
        StatusCode = dto.StatusCode,
        Project = dto.Project,
        DurationMs = dto.DurationMs
    };

    private static string? CombineException(string? message, string? stackTrace)
    {
        if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(stackTrace)) return null;
        return $"{message}\n{stackTrace}".Trim();
    }
}