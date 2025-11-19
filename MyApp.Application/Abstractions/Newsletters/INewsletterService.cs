using MyApp.Application.Abstractions.Newsletters.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Newsletters
{
    public interface INewsletterService
    {
        Task<Guid> CreateAsync(string subject, string content, Guid RestaurantId);
        Task UpdateAsync(Guid id, string subject, string content);
        Task DeleteAsync(Guid id);

        // فقط یک متد برای ارسال (همه چیز داخلش انجام بشه)
        Task SendNowAsync(Guid newsletterId);

        Task<NewsletterDto?> GetAsync(Guid id);
        Task<PagedResult<NewsletterListItemDto>> GetListAsync(int page = 1, int pageSize = 20);
    }
}