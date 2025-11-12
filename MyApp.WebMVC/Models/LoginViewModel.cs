using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
