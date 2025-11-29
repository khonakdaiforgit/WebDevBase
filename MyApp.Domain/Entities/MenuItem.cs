using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class MenuItem : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string Name { get;  set; }
        public string Description { get;  set; }
        public decimal Price { get;  set; }
        public string ImageUrl { get;  set; }
        public bool IsAvailable { get;  set; } = true;
        public Guid CategoryId { get; set; }
        public void ToggleAvailability() => IsAvailable = !IsAvailable;
    }
}
