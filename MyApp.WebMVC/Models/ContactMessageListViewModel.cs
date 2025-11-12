namespace MyApp.WebMVC.Models
{
    public class ContactMessageListViewModel
    {
        public List<ContactMessageViewModel> Messages { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool OnlyUnread { get; set; }
    }
}
