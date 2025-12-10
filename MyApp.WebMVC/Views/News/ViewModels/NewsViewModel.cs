namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class NewsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
