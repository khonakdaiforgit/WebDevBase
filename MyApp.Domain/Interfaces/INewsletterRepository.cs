using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface INewsletterRepository : IRepository<Newsletter>
    {
        Task<List<Newsletter>> GetSentAsync();
    }
}
