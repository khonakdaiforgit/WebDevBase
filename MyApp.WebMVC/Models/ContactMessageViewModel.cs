namespace MyApp.WebMVC.Models
{
    public class ContactMessageViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string ReportNumber { get; set; }
        public bool IsRead { get; set; }
    }

}
