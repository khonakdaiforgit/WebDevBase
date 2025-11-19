using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Repositories;

public class EmailSubscriberRepository : GenericRepository<EmailSubscriber>, IEmailSubscriberRepository
{
    public EmailSubscriberRepository(MongoDbContext context) : base(context) { }

    public async Task<EmailSubscriber?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _collection.Find(s => s.Email == email).FirstOrDefaultAsync(ct);

    public async Task<EmailSubscriber?> GetByUnsubscribeTokenAsync(string token, CancellationToken ct = default)
        => await _collection.Find(s => s.UnsubscribeToken == token).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<EmailSubscriber>> GetActiveByRestaurantAsync(Guid restaurantId, CancellationToken ct = default)
        => await _collection
            .Find(s => s.RestaurantId == restaurantId && s.IsActive)
            .ToListAsync(ct);
}