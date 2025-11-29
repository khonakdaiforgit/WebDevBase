namespace MyApp.WebMVC.Views.Menu.ViewModels
{
    public class MenuCategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Order { get; set; }
        public List<MenuItemViewModel> Items { get; set; } = new();

        // برای نمایش قیمت به دلار
        public string FormattedPrice(decimal price) => $"${price:N2}";
    }
}
