namespace MyApp.WebMVC.Views.ContactMessages.ViewModels
{
    public class ContactMessagesIndexViewModel
    {
        public List<ContactMessageViewModel> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public bool? OnlyUnread { get; set; }
    }
}
