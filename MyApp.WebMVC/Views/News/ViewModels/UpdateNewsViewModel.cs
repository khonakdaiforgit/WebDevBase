using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class UpdateNewsViewModel
    {
        public Guid Id { get; set; }
        [Required] public string Title { get; set; } = null!;
        [Required] public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsPublished { get; set; }
    }
}
