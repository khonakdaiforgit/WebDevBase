using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class LogEntry : IHasId<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "Info";
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public string? UserId { get; set; }
        public string? HashedIp { get; set; }
        public string? Route { get; set; }
        public string? Method { get; set; }
        public int? StatusCode { get; set; }
        public double? DurationMs { get; set; }
        public string Project { get; set; } = "API";
    }
}
