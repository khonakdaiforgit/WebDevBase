using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class EmailSubscriber : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string Email { get;  set; }
        public DateTime SubscribedAt { get;  set; } = DateTime.UtcNow;
        public bool IsActive { get;  set; } = true;
        public string UnsubscribeToken { get;  set; } = Guid.NewGuid().ToString();
        public Guid RestaurantId { get; set; }

        public void Unsubscribe() => IsActive = false;
        public void Confirm() => IsActive = true;
    }
}
