using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Galleries
{
    public interface IGalleryService
    {
        Task<Guid> UploadAsync(UploadGalleryItemDto dto, Guid callerUserId);
        Task UpdateAsync(UpdateGalleryItemDto dto, Guid callerUserId);
        Task DeleteAsync(Guid itemId, Guid callerUserId);
        Task HideAsync(Guid itemId, Guid callerUserId);
        Task ShowAsync(Guid itemId, Guid callerUserId);
        Task<PagedResult<GalleryItemDto>> GetForRestaurantAsync(Guid restaurantId, int page = 1, int pageSize = 20);
    }
}