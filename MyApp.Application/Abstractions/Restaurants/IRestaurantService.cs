using MyApp.Application.Abstractions.Restaurants.Dtos;

namespace MyApp.Application.Abstractions.Restaurants;

public interface IRestaurantService
{
    Task<Guid> CreateAsync(CreateRestaurantDto dto, Guid ownerUserId);
    Task UpdateAsync(UpdateRestaurantDto dto, Guid callerUserId);
    Task<RestaurantDto?> GetAsync(Guid restaurantId);
    Task UpdateLocationAsync(Guid restaurantId, double latitude, double longitude, Guid callerId);
    Task UpdateLogoAsync(UpdateLogoDto dto, Guid callerUserId);
    Task UpdateWorkingHoursAsync(UpdateWorkingHoursDto dto, Guid callerUserId);
    Task<bool> IsOwnerAsync(Guid restaurantId, Guid userId);
    Task<RestaurantDto?> GetByOwnerIdAsync(Guid ownerUserId);
    Task<RestaurantDto> GetMainRestaurantAsync();
    Task<PublicRestaurantDto> GetPublicInfo();
}