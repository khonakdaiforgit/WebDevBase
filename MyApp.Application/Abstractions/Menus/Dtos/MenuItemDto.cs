namespace MyApp.Application.Abstractions.Menus.Dtos
{
    public record MenuItemDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string ImageUrl,
        bool IsAvailable);
}