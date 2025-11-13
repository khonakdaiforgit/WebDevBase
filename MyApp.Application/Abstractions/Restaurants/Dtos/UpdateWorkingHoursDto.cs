using MyApp.Domain.ValueObjects;

namespace MyApp.Application.Abstractions.Restaurants.Dtos
{
    public record UpdateWorkingHoursDto(Guid RestaurantId, WorkingHours WorkingHours);
}