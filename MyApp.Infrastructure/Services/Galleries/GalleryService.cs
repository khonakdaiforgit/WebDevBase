using MyApp.Application.Abstractions.Galleries;
using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.Infrastructure.Services.Galleries
{
    public class GalleryService : IGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GalleryService(
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> UploadAsync(UploadGalleryItemDto dto)
        {

            var galleryItem = new GalleryItem
            {
                ImageUrl = dto.ImageUrl.Trim(),
                Caption = dto.Caption?.Trim() ?? string.Empty,
                UploadDate = DateTime.UtcNow,
                IsVisible = true // default: visible on website
            };

            await _unitOfWork.GalleryItems.AddAsync(galleryItem);
            await _unitOfWork.SaveChangesAsync();

            return galleryItem.Id;
        }

        public async Task UpdateAsync(UpdateGalleryItemDto dto)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("Gallery item not found.");


            if (dto.Caption is not null)
                item.Caption = dto.Caption.Trim();

            if (dto.IsVisible.HasValue)
                item.IsVisible = dto.IsVisible.Value;

            if (dto.ImageUrl.Any())
                item.ImageUrl = dto.ImageUrl;

            await _unitOfWork.GalleryItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid itemId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Gallery item not found.");


            await _unitOfWork.GalleryItems.DeleteAsync(itemId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task HideAsync(Guid itemId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Gallery item not found.");

            item.IsVisible = false;

            await _unitOfWork.GalleryItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ShowAsync(Guid itemId)
        {
            var item = await _unitOfWork.GalleryItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Gallery item not found.");


            item.IsVisible = true;

            await _unitOfWork.GalleryItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<GalleryItemDto>> GetForRestaurantAsync(
            int page = 1,
            int pageSize = 20)
        {

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

    }
}
