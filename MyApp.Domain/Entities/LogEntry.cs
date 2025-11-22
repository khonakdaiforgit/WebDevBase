using MyApp.Domain.Interfaces.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp.Domain.Entities
{
    public class LogEntry : IHasId<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "Info";
        public string? Message { get; set; }
        public string? Exception { get; set; }

        // پراپرتی‌های کمکی برای مپینگ راحت
        [JsonIgnore] // مهم: اینا در دیتابیس ذخیره نشن
        public string? ExceptionMessage => Exception != null
            ? System.Text.Json.JsonSerializer.Deserialize<JsonElement>(Exception)
                .GetProperty("Message").GetString()
            : null;

        [JsonIgnore]
        public string? StackTrace => Exception != null
            ? System.Text.Json.JsonSerializer.Deserialize<JsonElement>(Exception)
                .GetProperty("StackTrace").GetString()
            : null;

        [JsonIgnore]
        public string Path => Route ?? "-";
        public string? UserId { get; set; }
        public string? HashedIp { get; set; }
        public string? Route { get; set; }
        public string? Method { get; set; }
        public int? StatusCode { get; set; }
        public double? DurationMs { get; set; }
        public string Project { get; set; } = "API";
    }
}
