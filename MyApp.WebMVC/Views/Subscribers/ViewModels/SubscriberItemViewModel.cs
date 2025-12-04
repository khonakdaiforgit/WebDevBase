namespace MyApp.WebMVC.Views.Subscribers.ViewModels
{
    public class SubscriberItemViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime SubscribedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
