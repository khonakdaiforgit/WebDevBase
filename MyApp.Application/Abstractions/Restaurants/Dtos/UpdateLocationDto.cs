namespace MyApp.Application.Abstractions.Restaurants.Dtos;

public record UpdateLocationDto(
    Guid RestaurantId,
    double Latitude,
    double Longitude);