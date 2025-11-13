namespace MyApp.Application.Abstractions.Menus.Dtos
{
    public record UpdateMenuItemDto(
        Guid Id,
        string? Name,
        string? Description,
        decimal? Price,
        string? ImageUrl,
        bool? IsAvailable);
}