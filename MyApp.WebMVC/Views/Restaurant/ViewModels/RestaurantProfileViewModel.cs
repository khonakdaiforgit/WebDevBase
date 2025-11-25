using MyApp.Application.Abstractions.Restaurants.Dtos;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Restaurant.ViewModels
{
    public class RestaurantProfileViewModel
    {
        public Guid Id { get; set; }

        [Required] public string Name { get; set; } = null!;
        [Required] public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Phone] public string Phone { get; set; } = null!;
        [EmailAddress] public string Email { get; set; } = null!;
        public string? LogoUrl { get; set; }

        public Dictionary<string, TimeRangeDto> WorkingHours { get; init; } = new();
        // ←←← این دو تا جادویی رو اضافه کن
        public bool IsOpenNow => CalculateIsOpenNow();
        public string TodayHoursDisplay => GetTodayHoursDisplay();

        private bool CalculateIsOpenNow()
        {
            var now = DateTime.Now;
            var today = now.DayOfWeek.ToString();
            var timeNow = now.TimeOfDay;

            if (!WorkingHours.TryGetValue(today, out var range))
                return false;

            return timeNow >= range.Open && timeNow <= range.Close;
        }

        private string GetTodayHoursDisplay()
        {
            var today = DateTime.Today.DayOfWeek.ToString();
            return WorkingHours.TryGetValue(today, out var range)
                ? $"{range.Open:hh\\:mm} - {range.Close:hh\\:mm}"
                : "Closed";
        }
    }
}
