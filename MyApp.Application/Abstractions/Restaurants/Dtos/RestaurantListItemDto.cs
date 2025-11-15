using MyApp.Domain.ValueObjects;

namespace MyApp.Application.Abstractions.Restaurants.Dtos;

public record RestaurantListItemDto(
    Guid Id,
    string Name,
    string Address,
    Location Location,
    string? LogoUrl);