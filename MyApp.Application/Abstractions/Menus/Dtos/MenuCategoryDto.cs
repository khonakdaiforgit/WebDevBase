namespace MyApp.Application.Abstractions.Menus.Dtos
{
    public record MenuCategoryDto(
        Guid Id,
        string Name,
        int Order,
        IReadOnlyList<MenuItemDto> Items);
}