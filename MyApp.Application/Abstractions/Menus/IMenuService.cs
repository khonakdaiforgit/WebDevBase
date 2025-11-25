using MyApp.Application.Abstractions.Menus.Dtos;

namespace MyApp.Application.Abstractions.Menus
{
    public interface IMenuService
    {
        // دسته‌بندی
        Task<Guid> CreateCategoryAsync(CreateCategoryDto dto, Guid callerUserId);
        Task UpdateCategoryAsync(UpdateCategoryDto dto, Guid callerUserId);
        Task DeleteCategoryAsync(Guid categoryId, Guid callerUserId);
        Task<MenuCategoryDto?> GetCategoryAsync(Guid categoryId);
        Task<IReadOnlyList<MenuCategoryDto>> GetAllCategoriesWithItemsAsync();

        // آیتم
        Task<Guid> CreateItemAsync(CreateMenuItemDto dto, Guid callerUserId);
        Task UpdateItemAsync(UpdateMenuItemDto dto, Guid callerUserId);
        Task DeleteItemAsync(Guid itemId, Guid callerUserId);
        Task ToggleAvailabilityAsync(Guid itemId, Guid callerUserId);
    }
}