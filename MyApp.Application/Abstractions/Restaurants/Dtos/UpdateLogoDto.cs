namespace MyApp.Application.Abstractions.Restaurants.Dtos
{
    public record UpdateLogoDto(Guid RestaurantId, string LogoUrl);
}