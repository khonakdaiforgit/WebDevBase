using MyApp.Application.Abstractions.Restaurants.Dtos;

namespace MyApp.WebMVC.Views.Shared.ViewModels
{
    public record SharedLayoutViewModel
    {
        public string RestaurantName { get; init; } = "The Urban Bistro";
        public string? LogoUrl { get; init; } = "/img/logo_200px.png"; 
        public string FullAddress { get; init; } = "123 Downtown Street • New York, NY 10001";
        public string Phone { get; init; } = "+1 (555) 123-4567";
        public string Email { get; init; } = "info@urbanbistro.com";

        public bool IsOpenNow { get; init; } = true;

        public string TodayHoursDisplay { get; init; } = "11:00 AM – 11:00 PM";

        public Dictionary<string, TimeRangeDto> WorkingHours { get; init; } = new();

        public double Latitude { get; init; }
        public double Longitude { get; init; }

        public string YelpUrl { get; init; } = "https://www.yelp.com/biz/pearl-san-francisco";
        public string InstagramUrl { get; init; } = "https://www.instagram.com/pearl_6101/";
        public string? GoogleMapLink { get; set; }
    }
}
