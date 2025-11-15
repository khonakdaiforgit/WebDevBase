using MyApp.Domain.ValueObjects;

namespace MyApp.Application.Abstractions.Restaurants.Dtos;

public record CreateRestaurantDto(
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    string Phone,
    string Email,
    string? LogoUrl,
    WorkingHours WorkingHours);