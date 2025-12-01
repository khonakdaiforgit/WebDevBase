using MyApp.Application.Abstractions.Authorization;
using MyApp.Infrastructure.Common;

namespace MyApp.Infrastructure.Services.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthorizationService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsOwnerAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user?.IsProjectOwner == true;
        }

        public Task<bool> CanViewLogsAsync(Guid userId)
        {
            return IsOwnerAsync(userId); // فقط مالک
        }

        public async Task<bool> CanAccessRestaurantAsync(Guid userId, AccessLevel required = AccessLevel.StoreAdmin)
        {
            if (required == AccessLevel.Owner)
                return await IsOwnerAsync(userId);

            // StoreAdmin: مالک رستوران یا Owner پروژه
            var isRestaurantOwner = await _unitOfWork.Restaurants.IsOwnerAsync(userId);
            var isProjectOwner = await IsOwnerAsync(userId);

            return isRestaurantOwner || isProjectOwner;
        }
    }
}
