namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class NewsPublicViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!; // HTML از TinyMCE
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; } // اضافه شد
    }
}
