using MyApp.Application.Abstractions.Restaurants.Dtos;

namespace MyApp.Application.Abstractions.Restaurants;

public interface IRestaurantService
{
    Task<Guid> CreateAsync(CreateRestaurantDto dto);
    Task UpdateAsync(UpdateRestaurantDto dto);
    Task<RestaurantDto?> GetAsync(Guid restaurantId);
    Task UpdateLocationAsync(Guid restaurantId, double latitude, double longitude);
    Task UpdateLogoAsync(UpdateLogoDto dto);
    Task UpdateWorkingHoursAsync(UpdateWorkingHoursDto dto);
    Task<bool> IsOwnerAsync(Guid userId);
    Task<RestaurantDto?> GetByOwnerIdAsync(Guid ownerUserId);
    Task<RestaurantDto> GetMainRestaurantAsync();
    Task<PublicRestaurantDto> GetPublicInfo();
}