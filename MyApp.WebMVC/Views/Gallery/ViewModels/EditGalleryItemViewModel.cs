using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Gallery.ViewModels
{
    public class EditGalleryItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Current Photo URL")]
        public string CurrentImageUrl { get; set; } = string.Empty;

        [Display(Name = "Replace Photo")]
        public IFormFile? NewImageFile { get; set; }

        [Display(Name = "Caption")]
        public string? Caption { get; set; }

        [Display(Name = "Show on Website")]
        public bool IsVisible { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
