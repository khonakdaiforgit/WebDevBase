namespace MyApp.Application.Abstractions.Galleries.Dtos
{
    public record UploadGalleryItemDto(string ImageUrl, string Caption, Guid RestaurantId);
}