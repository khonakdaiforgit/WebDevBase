using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Models
{
    public class ContactMessageCreateViewModel
    {

        public string? ReportNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Subject must be between 2 and 100 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 1000 characters.")]
        public string Message { get; set; }
    }
}
