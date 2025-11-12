using MyApp.Domain.Entities;

namespace MyApp.Domain.Dtos
{
    public class UserProfileStats
    {
        public string UserId { get; set; }
        public DateTime? FirstLogin { get; set; }
        public int TotalLogins { get; set; }
        public int TotalVisits { get; set; }
        public int TotalFileViews { get; set; }
        public int TotalSuccessfulSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<DateTime, int> VisitsByDay { get; set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, decimal> RevenueByDay { get; set; } = new Dictionary<DateTime, decimal>();
        public List<LogEntry> RecentActivities { get; set; } = new List<LogEntry>();
        public List<LogEntry> RecentErrors { get; set; } = new List<LogEntry>();
    }
}
