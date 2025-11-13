using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface INewsletterRepository : IGenericRepository<Newsletter>
    {
        Task<PagedResult<Newsletter>> GetPagedByRestaurantAsync(
            Guid? restaurantId,
            NewsletterStatus? status = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);
    }
}
