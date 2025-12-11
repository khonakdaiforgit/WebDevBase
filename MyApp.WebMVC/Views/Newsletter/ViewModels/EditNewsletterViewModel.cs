using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Newsletter.ViewModels
{
    public class EditNewsletterViewModel
    {
        public Guid Id { get; set; }
        [Required] public string Subject { get; set; } = null!;
        [Required] public string Content { get; set; } = null!;
    }
}
