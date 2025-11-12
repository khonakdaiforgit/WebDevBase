using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces;
using MyApp.Infrastructure.Repositories.Common;

namespace MyApp.Infrastructure.Repositories
{
    public class ContactMessageRepository : MongoRepository<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
        {
        }

        public async Task<List<ContactMessage>> GetUnreadAsync()
        {
            var filter = Builders<ContactMessage>.Filter.Eq(x => x.IsRead, false);
            var sort = Builders<ContactMessage>.Sort.Descending(x => x.SentAt); // جدیدترین اول

            return await _collection
                .Find(filter)
                .Sort(sort)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var filter = Builders<ContactMessage>.Filter.Eq(x => x.Id, id);
            var update = Builders<ContactMessage>.Update.Set(x => x.IsRead, true);

            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
