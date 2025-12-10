using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class CreateNewsViewModel
    {
        [Required] public string Title { get; set; } = null!;
        [Required] public string Content { get; set; } = null!;
        public IFormFile? ImageFile { get; set; }
    }
}
