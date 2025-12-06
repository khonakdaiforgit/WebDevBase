namespace MyApp.WebMVC.Views.Gallery.ViewModels
{
    public class PublicGalleryItemViewModel
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
    }
}
