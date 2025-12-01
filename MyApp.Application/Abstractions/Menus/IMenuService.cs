using MyApp.Application.Abstractions.Menus.Dtos;

namespace MyApp.Application.Abstractions.Menus
{
    public interface IMenuService
    {
        // دسته‌بندی
        Task<Guid> CreateCategoryAsync(CreateCategoryDto dto);
        Task UpdateCategoryAsync(UpdateCategoryDto dto);
        Task DeleteCategoryAsync(Guid categoryId);
        Task<MenuCategoryDto?> GetCategoryAsync(Guid categoryId);
        Task<IReadOnlyList<MenuCategoryDto>> GetAllCategoriesWithItemsAsync();
        Task CategoryMoveOrderAsync(Guid categoryId, bool IsUp);

        // آیتم
        Task<MenuItemDto> GetItemAsync(Guid itemId);
        Task<Guid> CreateItemAsync(CreateMenuItemDto dto);
        Task UpdateItemAsync(UpdateMenuItemDto dto);
        Task DeleteItemAsync(Guid itemId);
        Task ToggleAvailabilityAsync(Guid itemId);
        Task ItemMoveOrderAsync(Guid categoryId, Guid itemId, bool IsUp);
    }
}