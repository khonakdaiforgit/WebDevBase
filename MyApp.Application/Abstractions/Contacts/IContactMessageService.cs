using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Contacts
{
    public interface IContactMessageService
    {
        Task<Guid> SubmitAsync(string name, string email, string message);
        Task MarkAsReadAsync(Guid messageId);
        Task<PagedResult<ContactMessageDto>> GetListAsync(bool? onlyUnread = null, int page = 1, int pageSize = 20);

        Task<int> GetUnreadCountAsync();
    }
}