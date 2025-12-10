namespace MyApp.WebMVC.Views.News.ViewModels
{
    public class NewsPublicListViewModel
    {
        public List<NewsPublicItemViewModel> News { get; set; } = new();
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
