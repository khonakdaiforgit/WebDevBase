using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface IMenuRepository : IRepository<MenuItem>
    {
        Task<List<MenuCategory>> GetMenuAsync(Guid restaurantId);
    }
}
