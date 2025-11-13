using Microsoft.Extensions.Configuration;
using MyApp.Application.Abstractions.Authorization;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Services.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;

        public AuthorizationService(
            IUserRepository userRepository,
            IRestaurantRepository restaurantRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<bool> IsOwnerAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.IsProjectOwner == true;
        }

        public Task<bool> CanViewLogsAsync(Guid userId)
        {
            return IsOwnerAsync(userId); // فقط مالک
        }

        public async Task<bool> CanAccessRestaurantAsync(Guid userId, Guid restaurantId, AccessLevel required = AccessLevel.StoreAdmin)
        {
            if (required == AccessLevel.Owner)
                return await IsOwnerAsync(userId);

            // StoreAdmin: مالک رستوران یا Owner پروژه
            var isRestaurantOwner = await _restaurantRepository.IsOwnerAsync(restaurantId, userId);
            var isProjectOwner = await IsOwnerAsync(userId);

            return isRestaurantOwner || isProjectOwner;
        }
    }
}
