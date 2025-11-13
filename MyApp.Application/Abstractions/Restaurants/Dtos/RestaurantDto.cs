using MyApp.Domain.ValueObjects;

namespace MyApp.Application.Abstractions.Restaurants.Dtos
{
    public record RestaurantDto(
        Guid Id,
        string Name,
        string Address,
        string Phone,
        string Email,
        string LogoUrl,
        WorkingHours WorkingHours);
}