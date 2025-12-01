using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities
{
    public class GalleryItem : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string ImageUrl { get;  set; }
        public string Caption { get;  set; }
        public DateTime UploadDate { get;  set; } = DateTime.UtcNow;
        public bool IsVisible { get;  set; } = true;

        public void Hide() => IsVisible = false;
        public void Show() => IsVisible = true;
    }
}
