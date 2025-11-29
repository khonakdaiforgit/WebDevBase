using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Menu.ViewModels
{
    public class UpdateCategoryViewModel
    {
        public Guid Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Range(0, 100)] public int Order { get; set; } = 0;
    }
}
