namespace MyApp.Application.Abstractions.Galleries.Dtos
{
    public record GalleryItemDto(
        Guid Id,
        string ImageUrl,
        string Caption,
        DateTime UploadDate,
        bool IsVisible);
}