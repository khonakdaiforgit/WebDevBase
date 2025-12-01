using AutoMapper;
using MyApp.Application.Abstractions.Menus;
using MyApp.Application.Abstractions.Menus.Dtos;
using MyApp.Application.Abstractions.Restaurants.Dtos;
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
        private readonly IMapper _mapper;

        public MenuService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        // ====================== Category Operations ======================

        public async Task<Guid> CreateCategoryAsync(CreateCategoryDto dto)
        {
            //await ValidateRestaurantAccessAsync(dto.RestaurantId, callerUserId);

            int lastOrder = await _unitOfWork.MenuCategories.MaxOrderAsync();

            var category = new MenuCategory
            {
                Name = dto.Name.Trim(),
                Order = ++lastOrder,
            };

            await _unitOfWork.MenuCategories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Id;
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.MenuCategories.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("Menu category not found.");


            category.Name = dto.Name.Trim();
            category.Order = dto.Order;

            await _unitOfWork.MenuCategories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _unitOfWork.MenuCategories.GetByIdAsync(categoryId);
            var items = await _unitOfWork.MenuItems.GetByCategoryIdAsync(categoryId);



            if (items.Any())
                throw new BadRequestException("Cannot delete a category that contains menu items.");

            await _unitOfWork.MenuCategories.DeleteAsync(categoryId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<MenuCategoryDto?> GetCategoryAsync(Guid categoryId)
        {
            var category = await _unitOfWork.MenuCategories.GetByIdAsync(categoryId);
            var items = await _unitOfWork.MenuItems.GetByCategoryIdAsync(categoryId);

            if (category is null) return null;


            var citems = items.Select(i => new MenuItemDto(
                i.Id,
                i.Name,
                i.Description,
                i.Price,
                i.ImageUrl,
                i.IsAvailable,
                i.Order
            )).ToList();

            return new MenuCategoryDto(
                category.Id,
                category.Name,
                category.Order,
                citems
                );
        }

        // <<< متد جدید و ضروری برای داشبورد >>>
        public async Task<IReadOnlyList<MenuCategoryDto>> GetAllCategoriesWithItemsAsync()
        {
            var categories = await _unitOfWork.MenuCategories.GetAllAsync();
            var AllItems = await _unitOfWork.MenuItems.GetAllAsync();

            // مرتب‌سازی بر اساس فیلد Order (برای Drag & Drop)
            var orderedCategories = categories
                .OrderBy(c => c.Order)
                .ToList();

            // مپینگ دستی — تمیز، سریع، بدون وابستگی به AutoMapper
            var dtos = orderedCategories.Select(category => new MenuCategoryDto(
                Id: category.Id,
                Name: category.Name.Trim(),
                Order: category.Order,
                Items: AllItems
                    .Where(c => c.CategoryId == category.Id)
                    .OrderBy(item => item.Order) // بعداً می‌تونی فیلد Order به MenuItem اضافه کنی
                    .Select(item => new MenuItemDto(
                        Id: item.Id,
                        Name: item.Name.Trim(),
                        Description: item.Description?.Trim() ?? string.Empty,
                        Price: item.Price,
                        ImageUrl: item.ImageUrl ?? string.Empty,
                        IsAvailable: item.IsAvailable,
                        Order: item.Order
                    ))
                    .ToList()
                    .AsReadOnly()
            ))
            .ToList()
            .AsReadOnly();

            return dtos;
        }

        public async Task CategoryMoveOrderAsync(Guid categoryId, bool IsUp)
        {
            var categories = await _unitOfWork.MenuCategories.GetAllAsync();

            var mainCategory = await _unitOfWork.MenuCategories.GetByIdAsync(categoryId);

            var orderWith = IsUp ? mainCategory.Order - 1 : mainCategory.Order + 1;


            var withCategory = categories.First(c => c.Order == orderWith);

            withCategory.Order = mainCategory.Order;
            mainCategory.Order = orderWith;

            await _unitOfWork.MenuCategories.UpdateAsync(withCategory);
            await _unitOfWork.MenuCategories.UpdateAsync(mainCategory);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ItemMoveOrderAsync(Guid categoryId, Guid itemId, bool IsUp)
        {
            var items = await _unitOfWork.MenuItems.GetAllAsync();
            items = items.Where(c => c.CategoryId == categoryId).ToList();

            var mainItem = await _unitOfWork.MenuItems.GetByIdAsync(itemId);

            var orderWith = IsUp ? mainItem.Order - 1 : mainItem.Order + 1;


            var withItem = items.First(c => c.Order == orderWith);

            withItem.Order = mainItem.Order;
            mainItem.Order = orderWith;

            await _unitOfWork.MenuItems.UpdateAsync(withItem);
            await _unitOfWork.MenuItems.UpdateAsync(mainItem);
            await _unitOfWork.SaveChangesAsync();
        }

        // ====================== Item Operations ======================

        public async Task<MenuItemDto> GetItemAsync(Guid itemId)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Menu item not found.");

            return new MenuItemDto(item.Id, item.Name, item.Description, item.Price, item.ImageUrl, item.IsAvailable, item.Order);
        }

        public async Task<Guid> CreateItemAsync(CreateMenuItemDto dto)
        {

            int lastOrder = await _unitOfWork.MenuItems.MaxOrderAsync();

            var item = new MenuItem
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                IsAvailable = dto.IsAvailable,
                Order = ++lastOrder
            };

            await _unitOfWork.MenuItems.AddAsync(item);


            //category.AddItem(item);

            //await _unitOfWork.MenuCategories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return item.Id;
        }

        public async Task UpdateItemAsync(UpdateMenuItemDto dto)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("Menu item not found.");

            //var category = await _unitOfWork.MenuCategories.GetByIdAsync(item.CategoryId)
            //    ?? throw new NotFoundException("Parent category not found.");

            //await ValidateRestaurantAccessAsync(category.RestaurantId, callerUserId);

            if (dto.Name is not null) item.Name = dto.Name.Trim();
            if (dto.Description is not null) item.Description = dto.Description.Trim();
            if (dto.Price.HasValue) item.Price = dto.Price.Value;
            if (dto.ImageUrl is not null) item.ImageUrl = dto.ImageUrl;
            if (dto.IsAvailable.HasValue) item.IsAvailable = dto.IsAvailable.Value;

            await _unitOfWork.MenuItems.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid itemId)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Menu item not found.");

            var category = await _unitOfWork.MenuCategories.GetByIdAsync(item.CategoryId)
                ?? throw new NotFoundException("Parent category not found.");


            await _unitOfWork.MenuItems.DeleteAsync(item.Id);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleAvailabilityAsync(Guid itemId)
        {
            var item = await _unitOfWork.MenuItems.GetByIdAsync(itemId)
                ?? throw new NotFoundException("Menu item not found.");

            var category = await _unitOfWork.MenuCategories.GetByIdAsync(item.CategoryId)
                ?? throw new NotFoundException("Parent category not found.");


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
