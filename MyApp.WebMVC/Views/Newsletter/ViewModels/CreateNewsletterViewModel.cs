using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Newsletter.ViewModels
{
    public class CreateNewsletterViewModel
    {
        [Required] public string Subject { get; set; } = null!;
        [Required] public string Content { get; set; } = null!;
    }
}
