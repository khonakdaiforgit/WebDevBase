using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Menu.ViewModels
{
    public class UpdateMenuItemViewModel
    {
        public Guid Id { get; set; }

        [Required] public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Range(0.01, 10000)] public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; } // برای تغییر عکس

        public bool IsAvailable { get; set; }
    }
}
