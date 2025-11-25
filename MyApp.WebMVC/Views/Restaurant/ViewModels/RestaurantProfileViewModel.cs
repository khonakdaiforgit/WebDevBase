using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Restaurant.ViewModels
{
    public class RestaurantProfileViewModel
    {
        public Guid Id { get; set; }

        [Required] public string Name { get; set; } = null!;
        [Required] public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Phone] public string Phone { get; set; } = null!;
        [EmailAddress] public string Email { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }
}
