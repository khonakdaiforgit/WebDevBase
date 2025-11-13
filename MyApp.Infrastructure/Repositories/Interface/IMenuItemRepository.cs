using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
        Task UpdateItemInCategoryAsync(Guid categoryId, MenuItem updatedItem, CancellationToken ct = default);
    }
}
