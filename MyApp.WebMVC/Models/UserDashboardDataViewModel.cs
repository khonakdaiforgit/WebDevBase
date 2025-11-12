using MyApp.Application.DTOs;

namespace MyApp.WebMVC.Models
{
    public class UserDashboardDataViewModel
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
        public List<LogEntryDto> RecentActivities { get; set; } = new List<LogEntryDto>();
        public List<LogEntryDto> RecentErrors { get; set; } = new List<LogEntryDto>();
    }
}
