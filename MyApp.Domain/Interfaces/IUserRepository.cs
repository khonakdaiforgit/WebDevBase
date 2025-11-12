using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task<User> GetByEmailAsync(string email);
        Task<bool> InsertedAdmin();
    }
}
