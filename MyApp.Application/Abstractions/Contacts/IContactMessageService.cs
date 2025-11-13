using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Contacts
{
    public interface IContactMessageService
    {
        Task<Guid> SubmitAsync(string name, string email, string message, Guid? restaurantId = null);
        Task MarkAsReadAsync(Guid messageId, Guid callerUserId);
        Task<PagedResult<ContactMessageDto>> GetListAsync(Guid? restaurantId = null, bool? onlyUnread = null, int page = 1, int pageSize = 20);
    }
}