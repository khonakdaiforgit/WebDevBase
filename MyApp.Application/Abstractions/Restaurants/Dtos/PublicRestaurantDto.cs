using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Abstractions.Restaurants.Dtos
{
    public record PublicRestaurantDto
    {
        public string Name { get; init; } = null!;
        public string? LogoUrl { get; init; }
        public string Address { get; init; } = null!;
        public string Phone { get; init; } = null!;
        public string Email { get; init; } = null!;
        public bool IsOpenNow { get; init; }
        public string TodayHoursDisplay { get; init; } = "Closed";
        public Dictionary<string, TimeRangeDto> WorkingHours { get; init; } = new();
    }
}
