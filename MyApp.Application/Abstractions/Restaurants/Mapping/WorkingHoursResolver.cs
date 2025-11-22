using AutoMapper;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Restaurants.Mapping
{
    // فقط این یک کلاس کافیه — برای همه DTOها کار می‌کنه!
    public class WorkingHoursResolver<TDestination>
        : IValueResolver<Restaurant, TDestination, Dictionary<string, TimeRangeDto>>
    {
        public Dictionary<string, TimeRangeDto> Resolve(
            Restaurant source,
            TDestination destination,
            Dictionary<string, TimeRangeDto> destMember,
            ResolutionContext context)
        {
            if (source.WorkingHours?.DailyHours == null)
                return new Dictionary<string, TimeRangeDto>();

            return source.WorkingHours.DailyHours.ToDictionary(
                kvp => kvp.Key,
                kvp => new TimeRangeDto(kvp.Value.Open, kvp.Value.Close)
            );
        }
    }
}
