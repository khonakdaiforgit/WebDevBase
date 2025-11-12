using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        Task<List<News>> GetPublishedAsync(int count = 10);
    }
}
