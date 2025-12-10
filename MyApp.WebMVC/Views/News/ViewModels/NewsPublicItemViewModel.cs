namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class NewsPublicItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = string.Empty; // خلاصه کوتاه
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
    }
}
