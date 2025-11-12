using MongoDB.Driver;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Infrastructure.Repositories.Common
{
    // Infrastructure/Repositories/MongoRepository.cs
    public class MongoRepository<T> : IRepository<T> where T : class, IHasId<Guid>
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoClient client, MongoDbSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            var collectionName = typeof(T).Name + "s"; // GalleryItem → GalleryItems
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> GetAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
