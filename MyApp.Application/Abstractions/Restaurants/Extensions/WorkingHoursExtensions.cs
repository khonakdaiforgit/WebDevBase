using MyApp.Application.Abstractions.Restaurants.Dtos;

namespace MyApp.Application.Abstractions.Restaurants.Extensions
{
    public static class WorkingHoursExtensions
    {
        public static Dictionary<string, (TimeSpan open, TimeSpan close)> ToDomainDictionary(
            this Dictionary<string, TimeRangeDto> dto)
        {
            return dto.ToDictionary(
                kvp => kvp.Key,
                kvp => (kvp.Value.Open, kvp.Value.Close)
            );
        }
    }
}
