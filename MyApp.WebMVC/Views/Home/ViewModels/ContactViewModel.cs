using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Home.ViewModels
{
    public class ContactViewModel
    {

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please write your message")]
        public string Message { get; set; } = string.Empty;
    }
}
