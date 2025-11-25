namespace MyApp.Application.Abstractions.Restaurants.Dtos;

public record UpdateRestaurantDto(
    Guid Id,
    string? Name,
    string? Address,
    double? Latitude,
    double? Longitude,
    string? Phone,
    string? Email,
    string? LogoUrl,
    Dictionary<string, TimeRangeDto> WorkingHours
);