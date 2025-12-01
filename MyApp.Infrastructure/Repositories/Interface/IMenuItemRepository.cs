using MyApp.Application.Abstractions.Menus.Dtos;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<IReadOnlyList<MenuItem>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);
        Task<int> MaxOrderAsync(CancellationToken ct = default);
        Task ToggleAvailabilityAsync(Guid itemId, CancellationToken ct = default);
    }
}
