namespace MyApp.Application.Abstractions.Restaurants.Dtos
{
    public record RestaurantListItemDto(
        Guid Id,
        string Name,
        string Address,
        string LogoUrl);
}