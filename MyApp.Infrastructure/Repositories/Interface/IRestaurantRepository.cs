using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IRestaurantRepository : IGenericRepository<Restaurant>
    {
        Task<IReadOnlyList<Restaurant>> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
        Task<Restaurant> GetMain();
        Task<bool> IsOwnerAsync(Guid restaurantId, Guid userId, CancellationToken ct = default);
    }
}
