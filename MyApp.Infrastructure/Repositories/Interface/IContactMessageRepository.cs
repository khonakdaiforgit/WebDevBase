using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;
using System.Linq.Expressions;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IContactMessageRepository : IGenericRepository<ContactMessage>
    {
        Task<PagedResult<ContactMessage>> GetPagedAsync(
            bool? onlyUnread = null,
            Expression<Func<ContactMessage, object>>? orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);
    }
}
