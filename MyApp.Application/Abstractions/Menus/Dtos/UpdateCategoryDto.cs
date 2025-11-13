namespace MyApp.Application.Abstractions.Menus.Dtos
{
    public record UpdateCategoryDto(Guid Id, string Name, int Order);
}