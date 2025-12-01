namespace MyApp.Application.Abstractions.Galleries.Dtos
{
    public record UpdateGalleryItemDto(Guid Id, string? Caption, string ImageUrl, bool? IsVisible);
}