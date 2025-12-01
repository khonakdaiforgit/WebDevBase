using AutoMapper;
using MyApp.Application.Abstractions.Restaurants;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.Application.Abstractions.Restaurants.Extensions;
using MyApp.Application.Abstractions.Users;
using MyApp.Domain.ValueObjects;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.Infrastructure.Services.Restaurant
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;


        public RestaurantService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUserService;
        }

        public async Task<Guid> CreateAsync(CreateRestaurantDto dto)
        {

            var restaurant = new Domain.Entities.Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                LogoUrl = dto.LogoUrl,
                OwnerUserId = _currentUser.UserId.Value
            };

            restaurant.SetLocation(dto.Latitude, dto.Longitude);

            // استفاده از متد استاتیک Create
            restaurant.WorkingHours = WorkingHours.Create(
                dto.WorkingHours.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (kvp.Value.Open, kvp.Value.Close)
                )
            );

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();

            return restaurant.Id;
        }

        public async Task UpdateAsync(UpdateRestaurantDto dto)
        {
            var restaurant = await GetRestaurantOrThrowAsync(dto.Id);

            if (dto.Name != null) restaurant.Name = dto.Name;
            if (dto.Address != null) restaurant.Address = dto.Address;
            if (dto.Phone != null) restaurant.Phone = dto.Phone;
            if (dto.Email != null) restaurant.Email = dto.Email;
            if (dto.LogoUrl != null) restaurant.UpdateLogo(dto.LogoUrl);
            //if (dto.WorkingHours is { Count: > 0 })
            //{
            //    restaurant.WorkingHours = WorkingHours.Create(dto.WorkingHours.ToDomainDictionary());
            //}
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

        public async Task UpdateLocationAsync(Guid restaurantId, double latitude, double longitude)
        {
            var restaurant = await GetRestaurantOrThrowAsync(restaurantId);
            restaurant.SetLocation(latitude, longitude);

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateLogoAsync(UpdateLogoDto dto)
        {
            var restaurant = await GetRestaurantOrThrowAsync(dto.RestaurantId);
            restaurant.UpdateLogo(dto.LogoUrl);

            await _unitOfWork.Restaurants.AddAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateWorkingHoursAsync(UpdateWorkingHoursDto dto)
        {
            var restaurant = await GetRestaurantOrThrowAsync(dto.RestaurantId);

            var domainHours = dto.WorkingHours?.ToDictionary(
               kvp => kvp.Key,
               kvp => (Open: kvp.Value.Open, Close: kvp.Value.Close)
            ) ?? new Dictionary<string, (TimeSpan Open, TimeSpan Close)>();

            var newWorkingHours = WorkingHours.Create(domainHours);
            restaurant.WorkingHours = newWorkingHours;

            await _unitOfWork.Restaurants.UpdateAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsOwnerAsync(Guid userId)
        {
            return await _unitOfWork.Restaurants.IsOwnerAsync(userId);
        }

        public async Task<RestaurantDto?> GetByOwnerIdAsync(Guid ownerUserId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByOwnerAsync(ownerUserId);

            return restaurant == null ? null : _mapper.Map<RestaurantDto>(restaurant.FirstOrDefault());
        }

        // --- متد کمکی ---
        private async Task<Domain.Entities.Restaurant> GetRestaurantOrThrowAsync(Guid restaurantId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId, CancellationToken.None);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant with ID {restaurantId} not found.");

            return restaurant;
        }

        public async Task<RestaurantDto> GetMainRestaurantAsync()
        {
            var restaurant = await _unitOfWork.Restaurants.GetMain();

            return _mapper.Map<RestaurantDto>(restaurant);
        }
        public async Task<PublicRestaurantDto> GetPublicInfo()
        {
            var restaurant = await _unitOfWork.Restaurants.GetMain();

            return _mapper.Map<PublicRestaurantDto>(restaurant);
        }

       
    }
}
