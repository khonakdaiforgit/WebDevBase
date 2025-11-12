using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface IContactMessageRepository : IRepository<ContactMessage>
    {
        Task<List<ContactMessage>> GetUnreadAsync();
        Task MarkAsReadAsync(Guid id);
    }
}
