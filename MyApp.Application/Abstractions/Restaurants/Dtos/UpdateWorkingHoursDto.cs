namespace MyApp.Application.Abstractions.Restaurants.Dtos

{
    public record UpdateWorkingHoursDto(
        Guid RestaurantId,
        Dictionary<string, (TimeSpan Open, TimeSpan Close)> WorkingHours // تغییر نوع
    );
}