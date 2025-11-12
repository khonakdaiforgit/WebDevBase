using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces;
using MyApp.Infrastructure.Repositories.Common;

namespace MyApp.Infrastructure.Repositories;

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
    {
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken)
    {
        var filter = Builders<User>.Filter.Eq(x => x.RefreshToken, refreshToken);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> InsertedAdmin()
    {
        var filter = Builders<User>.Filter.Eq(x => x.Role, UserRole.Admin);
        return await _collection.Find(filter).CountDocumentsAsync() > 0;

    }
}
