using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Galleries
{
    public interface IGalleryService
    {
        Task<Guid> UploadAsync(UploadGalleryItemDto dto);
        Task UpdateAsync(UpdateGalleryItemDto dto);
        Task DeleteAsync(Guid itemId);
        Task HideAsync(Guid itemId);
        Task ShowAsync(Guid itemId);
        Task<PagedResult<GalleryItemDto>> GetForRestaurantAsync(int page = 1, int pageSize = 20);
    }
}