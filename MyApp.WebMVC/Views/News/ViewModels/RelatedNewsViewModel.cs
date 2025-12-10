namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class RelatedNewsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
    }
}
