using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class LogEntry : IHasId<Guid>
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Level { get; set; } = "Info"; // Info, Warning, Error, Critical

        public string Message { get; set; } = string.Empty;

        public string? ExceptionMessage { get; set; }

        public string? StackTrace { get; set; }

        public string UserId { get; set; } = "Anonymous"; // User ID یا Anonymous

        public string HashedIp { get; set; } = string.Empty; // IP هش‌شده

        public string UserAgent { get; set; } = string.Empty;

        public string Method { get; set; } = string.Empty; // GET, POST, etc.

        public string Path { get; set; } = string.Empty;

        public int StatusCode { get; set; } = 0;

        public string? Source { get; set; } // Start, End, Error

        public string Project { get; set; } = "Unknown"; // "API" یا "MVC"
    }
}
