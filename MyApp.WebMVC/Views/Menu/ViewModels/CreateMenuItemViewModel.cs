using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Menu.ViewModels
{
    public class CreateMenuItemViewModel
    {
        public Guid CategoryId { get; set; }

        [Required] public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Range(0.01, 10000)] public decimal Price { get; set; } = 9.99m;

        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; } // برای آپلود

        public bool IsAvailable { get; set; } = true;
    }
}
