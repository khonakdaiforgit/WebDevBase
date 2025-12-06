namespace MyApp.WebMVC.Views.Menu.ViewModels
{
    public class PublicMenuCategoryViewModel
    {
        public string Name { get; set; } = string.Empty;
        public List<PublicMenuItemViewModel> Items { get; set; } = new();
    }
}
