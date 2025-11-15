using AutoMapper;
using MyApp.Application.Abstractions.Restaurants;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.Infrastructure.Services.Restaurant
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RestaurantService(
            IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(CreateRestaurantDto dto, Guid ownerUserId)
        {
            var restaurant = new Domain.Entities.Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                LogoUrl = dto.LogoUrl,
                OwnerUserId = ownerUserId
            };

            restaurant.SetLocation(dto.Latitude, dto.Longitude);
            restaurant.UpdateHours(dto.WorkingHours);

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();

            return restaurant.Id;
        }

        public async Task UpdateAsync(UpdateRestaurantDto dto, Guid callerUserId)
        {
            var restaurant = await GetRestaurantOrThrowAsync(dto.Id, callerUserId);

            if (dto.Name != null) restaurant.Name = dto.Name;
            if (dto.Address != null) restaurant.Address = dto.Address;
            if (dto.Phone != null) restaurant.Phone = dto.Phone;
            if (dto.Email != null) restaurant.Email = dto.Email;
            if (dto.LogoUrl != null) restaurant.UpdateLogo(dto.LogoUrl);
            if (dto.WorkingHours != null) restaurant.UpdateHours(dto.WorkingHours);
            if (dto.Latitude.HasValue && dto.Longitude.HasValue)
                restaurant.SetLocation(dto.Latitude.Value, dto.Longitude.Value);

            await _unitOfWork.Restaurants.UpdateAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<RestaurantDto?> GetAsync(Guid restaurantId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
            return restaurant == null ? null : _mapper.Map<RestaurantDto>(restaurant);
        }

        public async Task UpdateLocationAsync(Guid restaurantId, double latitude, double longitude, Guid callerId)
        {
            var restaurant = await GetRestaurantOrThrowAsync(restaurantId, callerId);
            restaurant.SetLocation(latitude, longitude);

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateLogoAsync(UpdateLogoDto dto, Guid callerUserId)
        {
            var restaurant = await GetRestaurantOrThrowAsync(dto.RestaurantId, callerUserId);
            restaurant.UpdateLogo(dto.LogoUrl);

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateWorkingHoursAsync(UpdateWorkingHoursDto dto, Guid callerUserId)
        {
            var restaurant = await GetRestaurantOrThrowAsync(dto.RestaurantId, callerUserId);
            restaurant.UpdateHours(dto.WorkingHours);

            await _unitOfWork.Restaurants.UpdateAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsOwnerAsync(Guid restaurantId, Guid userId)
        {
            return await _unitOfWork.Restaurants.IsOwnerAsync(restaurantId, userId);
        }

        // --- متد کمکی ---
        private async Task<Domain.Entities.Restaurant> GetRestaurantOrThrowAsync(Guid restaurantId, Guid callerUserId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId, CancellationToken.None);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant with ID {restaurantId} not found.");

            if (restaurant.OwnerUserId != callerUserId)
                throw new ForbiddenException("You are not the owner of this restaurant.");

            return restaurant;
        }
    }
}
