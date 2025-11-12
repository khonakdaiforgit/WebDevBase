using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class MenuCategory : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string Name { get;  set; }
        public int Order { get;  set; }
        public Guid RestaurantId { get;  set; }
        public List<MenuItem> Items { get;  set; } = new();

        public void AddItem(MenuItem item) => Items.Add(item);
        public void RemoveItem(Guid itemId) => Items.RemoveAll(x => x.Id == itemId);
    }
}
