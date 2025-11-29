namespace MyApp.Application.Abstractions.Restaurants.Dtos

{
    // جدید و درست
    public record UpdateWorkingHoursDto(
        Guid RestaurantId,
        Dictionary<string, DayHoursDto> WorkingHours
    );

    public record DayHoursDto(TimeSpan Open, TimeSpan Close);
}