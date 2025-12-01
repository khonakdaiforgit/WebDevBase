namespace MyApp.WebMVC.Views.Gallery.ViewModels
{
    public class GalleryIndexViewModel
    {
        public List<GalleryItemViewModel> Items { get; set; } = new();
        public int TotalCount => Items.Count;
    }
}
