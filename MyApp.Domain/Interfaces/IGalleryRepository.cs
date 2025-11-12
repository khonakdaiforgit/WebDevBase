using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface IGalleryRepository : IRepository<GalleryItem>
    {
        Task<List<GalleryItem>> GetVisibleAsync();
    }
}
