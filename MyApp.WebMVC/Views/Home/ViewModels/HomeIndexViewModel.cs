namespace MyApp.WebMVC.Views.Home.ViewModels
{
    public class HomeIndexViewModel
    {
        public string RestaurantName { get; init; } = null!;
        public string? LogoUrl { get; init; }
        public string Address { get; init; } = null!;
        public string Phone { get; init; } = null!;
        public bool IsOpenNow { get; init; }
        public string TodayHoursDisplay { get; init; } = "Closed";
        public string StatusText => IsOpenNow ? "We're Open Now" : "Closed • Opens Soon";
        public string StatusBadgeClass => IsOpenNow ? "bg-success" : "bg-danger";
    }
}
