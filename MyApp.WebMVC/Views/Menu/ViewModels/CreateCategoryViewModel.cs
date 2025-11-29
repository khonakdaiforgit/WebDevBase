using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Menu.ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required] public string Name { get; set; } = null!;
        [Range(0, 100)] public int Order { get; set; } = 0;
    }
}
