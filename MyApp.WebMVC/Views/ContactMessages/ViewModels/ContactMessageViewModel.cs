namespace MyApp.WebMVC.Views.ContactMessages.ViewModels
{
    public class ContactMessageViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }

        // برای نمایش بهتر در ویو
        public string ShortMessage => Message.Length > 120
            ? Message.Substring(0, 117) + "..."
            : Message;

        public string SentAtDisplay => SentAt.ToString("MMM dd, yyyy • HH:mm");

        public string StatusBadge => IsRead ? "Read" : "New";
        public string StatusClass => IsRead ? "bg-success-subtle text-success" : "bg-warning-subtle text-warning fw-bold";
    }
}
