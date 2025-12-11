namespace MyApp.WebMVC.Views.Newsletter.ViewModels
{
    public class NewsletterViewModel
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = null!;
        public DateTime? SentAt { get; set; }
        public NewsletterStatus Status { get; set; }
    }
}
