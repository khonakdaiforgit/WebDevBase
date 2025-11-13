namespace MyApp.Application.Abstractions.Menus.Dtos
{
    public record CreateMenuItemDto(
        string Name,
        string Description,
        decimal Price,
        string ImageUrl,
        bool IsAvailable,
        Guid CategoryId);
}