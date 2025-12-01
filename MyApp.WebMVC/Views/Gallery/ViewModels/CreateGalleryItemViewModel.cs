using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Gallery.ViewModels
{
    public class CreateGalleryItemViewModel
    {
        [Required]
        public IFormFile ImageFile { get; set; } = null!;

        [Display(Name = "Caption (Optional)")]
        public string? Caption { get; set; }

        [Display(Name = "Show on website immediately")]
        public bool IsVisible { get; set; } = true;
    }
}