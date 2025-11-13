using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(MongoDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _collection.Find(x => x.Email == email).FirstOrDefaultAsync(ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        => await _collection.Find(x => x.RefreshToken == refreshToken).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default)
        => await _collection.Find(x => x.Role == role).ToListAsync(ct);
}