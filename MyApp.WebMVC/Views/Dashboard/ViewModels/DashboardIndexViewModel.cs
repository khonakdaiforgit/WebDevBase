namespace MyApp.WebMVC.Views.Dashboard.ViewModels
{
    public class DashboardIndexViewModel
    {
        public Guid RestaurantId { get; init; }
        public string Name { get; init; } = null!;
        public string? LogoUrl { get; init; }
        public bool IsOpenNow { get; init; }
        public string TodayHoursDisplay { get; init; } = "Closed";
        public int TotalVisitsToday { get; init; } = 1248;
        public int ActiveSubscribers { get; init; } = 842;
    }
}
