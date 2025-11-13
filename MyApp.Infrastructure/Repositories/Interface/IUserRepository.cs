using MyApp.Domain.Entities;
using MyApp.Infrastructure.Repositories.Interface.Common;

namespace MyApp.Infrastructure.Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default);
    }
}
