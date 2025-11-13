using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class Newsletter : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string Subject { get;  set; }
        public string Content { get;  set; } // HTML or Markdown
        public DateTime? SentAt { get;  set; }
        public Guid SentByUserId { get;  set; }
        public NewsletterStatus Status { get;  set; } = NewsletterStatus.Draft;
        public Guid RestaurantId { get; set; }

        public void Send(Guid userId)
        {
            Status = NewsletterStatus.Sent;
            SentAt = DateTime.UtcNow;
            SentByUserId = userId;
        }

        public void Schedule() => Status = NewsletterStatus.Scheduled;
    }
}

public enum NewsletterStatus { Draft, Scheduled, Sent, Failed }