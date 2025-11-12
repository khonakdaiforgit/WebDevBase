using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class News : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string Title { get;  set; }
        public string Content { get;  set; }
        public string ImageUrl { get;  set; }
        public DateTime PublishDate { get;  set; } = DateTime.UtcNow;
        public bool IsPublished { get;  set; }

        public void Publish() => IsPublished = true;
        public void Unpublish() => IsPublished = false;
    }
}
