using MyApp.Application.Abstractions.Galleries;
using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.Infrastructure.Services.Galleries
{
    public class GalleryService : IGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GalleryService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Guid> UploadAsync(UploadGalleryItemDto dto, Guid callerUserId)
        {
            await ValidateRestaurantAccessAsync(dto.RestaurantId, callerUserId);

            var galleryItem = new GalleryItem
            {
                ImageUrl = dto.ImageUrl.Trim(),
                Caption = dto.Caption?.Trim() ?? string.Empty,
                RestaurantId = dto.RestaurantId,
                UploadDate = DateTime.UtcNow,
                IsVisible = true // default: visible on website
            };

            await _unitOfWork.GalleryItems.AddAsync(galleryItem);
            await _unitOfWork.SaveChangesAsync();

            return galleryItem.Id;
        }

        public async Task UpdateAsync(UpdateGalleryItemDto dto, Guid callerUserId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("Gallery item not found.");

            await ValidateRestaurantAccessAsync(item.RestaurantId, callerUserId);

            if (dto.Caption is not null)
                item.Caption = dto.Caption.Trim();

            if (dto.IsVisible.HasValue)
                item.IsVisible = dto.IsVisible.Value;

            await _unitOfWork.GalleryItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid itemId, Guid callerUserId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Gallery item not found.");

            await ValidateRestaurantAccessAsync(item.RestaurantId, callerUserId);

            await _unitOfWork.GalleryItems.DeleteAsync(itemId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task HideAsync(Guid itemId, Guid callerUserId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Gallery item not found.");

            await ValidateRestaurantAccessAsync(item.RestaurantId, callerUserId);

            item.IsVisible = false;

            await _unitOfWork.GalleryItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ShowAsync(Guid itemId, Guid callerUserId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Gallery item not found.");

            await ValidateRestaurantAccessAsync(item.RestaurantId, callerUserId);

            item.IsVisible = true;

            await _unitOfWork.GalleryItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<GalleryItemDto>> GetForRestaurantAsync(
            Guid restaurantId,
            int page = 1,
            int pageSize = 20)
        {
            await ValidateRestaurantAccessAsync(restaurantId, _currentUser.UserId.Value);

            var pagedResult = await _unitOfWork.GalleryItems.GetPagedAsync(
                page: page,
                pageSize: pageSize);

            var dtos = pagedResult.Items.Select(g => new GalleryItemDto(
                g.Id,
                g.ImageUrl,
                g.Caption,
                g.UploadDate,
                g.IsVisible
            )).ToList();

            return new PagedResult<GalleryItemDto>(
                dtos,
                pagedResult.TotalCount,
                page,
                pageSize);
        }

        // Private helpers
        private async Task ValidateRestaurantAccessAsync(Guid restaurantId, Guid userId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId)
                ?? throw new NotFoundException("Restaurant not found.");

            // Single-restaurant system: all authenticated users have access
            // Future: add ownership/role check
            return;
        }
    }
}
