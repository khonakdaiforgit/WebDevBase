namespace MyApp.WebMVC.Views.Newsletter.ViewModels
{
    public class NewsletterDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? SentAt { get; set; }
        public NewsletterStatus Status { get; set; }
        public Guid SentByUserId { get; set; }
    }
}
