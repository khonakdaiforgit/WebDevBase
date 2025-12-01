using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class ContactMessage : IHasId<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public void MarkAsRead() => IsRead = true;
    }
}
