using System.Text.Json.Serialization;

namespace MyApp.Application.Abstractions.Logging.Dtos
{
    public record LogEntryDto(
     Guid Id,
     DateTime Timestamp,
     string Level, // Info, Warning, Error
     string Message,

     [property: JsonPropertyName("exception")]
    string? ExceptionMessage,

     [property: JsonPropertyName("stackTrace")]
    string? StackTrace,

     string UserId,
     string? HashedIp,
     string? Method,
     string? Path,
     int StatusCode,
     string Project,
     double? DurationMs = null)
    {
        // برای نمایش بهتر در UI
        public string LevelBadge => Level switch
        {
            "Info" => "bg-info",
            "Warning" => "bg-warning",
            "Error" => "bg-danger",
            _ => "bg-secondary"
        };

        public string Duration => DurationMs.HasValue
            ? $"{DurationMs.Value:F1}ms"
            : "-";

        public string ShortPath => Path?.Length > 50
            ? "..." + Path[^47..]
            : Path ?? "-";
    }
}