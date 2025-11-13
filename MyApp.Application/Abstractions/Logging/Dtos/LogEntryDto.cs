namespace MyApp.Application.Abstractions.Logging.Dtos
{
    public record LogEntryDto(
        Guid Id,
        DateTime Timestamp,
        string Level,
        string Message,
        string? ExceptionMessage,
        string? StackTrace,
        string UserId,
        string HashedIp,
        string UserAgent,
        string Method,
        string Path,
        int StatusCode,
        string? Source,
        string Project);
}