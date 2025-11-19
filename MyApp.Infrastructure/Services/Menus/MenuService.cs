using MyApp.Application.Abstractions.Menus;
using MyApp.Application.Abstractions.Menus.Dtos;
using MyApp.Application.Abstractions.Users;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.Infrastructure.Services.Menus
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public MenuService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        // ====================== Category Operations ======================

        public async Task<Guid> CreateCategoryAsync(CreateCategoryDto dto, Guid callerUserId)
        {
            await ValidateRestaurantAccessAsync(dto.RestaurantId, callerUserId);

            var category = new MenuCategory
            {
                Name = dto.Name.Trim(),
                Order = dto.Order,
                RestaurantId = dto.RestaurantId
            };

            await _unitOfWork.MenuCategories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Id;
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto dto, Guid callerUserId)
        {
            var category = await _unitOfWork.MenuCategories.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("Menu category not found.");

            await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            category.Name = dto.Name.Trim();
            category.Order = dto.Order;

            await _unitOfWork.MenuCategories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId, Guid callerUserId)
        {
            var category = await _unitOfWork.MenuCategories.GetWithItemsAsync(categoryId)
                ?? throw new NotFoundException("Menu category not found.");

            await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            if (category.Items.Any())
                throw new BadRequestException("Cannot delete a category that contains menu items.");

            await _unitOfWork.MenuCategories.DeleteAsync(categoryId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<MenuCategoryDto?> GetCategoryAsync(Guid categoryId)
        {
            var category = await _unitOfWork.MenuCategories.GetWithItemsAsync(categoryId);
            if (category is null) return null;

            await ValidateRestaurantAccessAsync(category.RestaurantId, _currentUser.UserId.Value);

            var items = category.Items.Select(i => new MenuItemDto(
                i.Id,
                i.Name,
                i.Description,
                i.Price,
                i.ImageUrl,
                i.IsAvailable
            )).ToList();

            return new MenuCategoryDto(
                category.Id,
                category.Name,
                category.Order,
                items);
        }

        // ====================== Item Operations ======================

        public async Task<Guid> CreateItemAsync(CreateMenuItemDto dto, Guid callerUserId)
        {
            var category = await _unitOfWork.MenuCategories.GetByIdAsync(dto.CategoryId)
                ?? throw new NotFoundException("Menu category not found.");

            await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            var item = new MenuItem
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                IsAvailable = dto.IsAvailable
            };

            category.AddItem(item);

            await _unitOfWork.MenuCategories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return item.Id;
        }

        public async Task UpdateItemAsync(UpdateMenuItemDto dto, Guid callerUserId)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("Menu item not found.");

            var category = await _unitOfWork.MenuCategories.GetByIdAsync(item.CategoryId)
                ?? throw new NotFoundException("Parent category not found.");

            await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            if (dto.Name is not null) item.Name = dto.Name.Trim();
            if (dto.Description is not null) item.Description = dto.Description.Trim();
            if (dto.Price.HasValue) item.Price = dto.Price.Value;
            if (dto.ImageUrl is not null) item.ImageUrl = dto.ImageUrl;
            if (dto.IsAvailable.HasValue) item.IsAvailable = dto.IsAvailable.Value;

            await _unitOfWork.MenuItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid itemId, Guid callerUserId)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Menu item not found.");

            var category = await _unitOfWork.MenuCategories.GetByIdAsync(item.CategoryId)
                ?? throw new NotFoundException("Parent category not found.");

            await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            category.RemoveItem(itemId);
            await _unitOfWork.MenuCategories.UpdateAsync(category);

            await _unitOfWork.MenuItems.DeleteAsync(itemId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleAvailabilityAsync(Guid itemId, Guid callerUserId)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Menu item not found.");

            var category = await _unitOfWork.MenuCategories.GetByIdAsync(item.CategoryId)
                ?? throw new NotFoundException("Parent category not found.");

            await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            item.ToggleAvailability();

            await _unitOfWork.MenuItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        // ====================== Private Helpers ======================

        private async Task ValidateRestaurantAccessAsync(Guid restaurantId, Guid userId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId)
                ?? throw new NotFoundException("Restaurant not found.");

            // In single-restaurant mode: all authenticated users have access
            // Future: check ownership or role
            return;
        }

        private async Task<Guid> GetCurrentUserRestaurantIdAsync()
        {
            var restaurants = await _unitOfWork.Restaurants.GetAllAsync();
            return restaurants.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No restaurant found in the system.");
        }
    }
}
