using MongoDB.Driver;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure
{
    public class MongoIndexing
    {
        private readonly IMongoClient _client;
        private readonly string _databaseName;
        public MongoIndexing(IMongoClient client, MongoDbSettings settings)
        {
            _client = client;
            _databaseName = settings.DatabaseName;
        }

        public void RegisterIndexing()
        {
            var logEntryIndexKeys = Builders<LogEntry>.IndexKeys
                .Ascending(x => x.Timestamp)
                .Ascending(x => x.UserId)
                .Ascending(x => x.Level)
                .Ascending(x => x.Project)
                .Text(x => x.Path)
                .Text(x => x.Message);
            CreateIndex(logEntryIndexKeys, true);

            var logEntryIndexKeyTimestamp = Builders<LogEntry>.IndexKeys
                .Ascending(x => x.Timestamp);
            CreateIndex(logEntryIndexKeyTimestamp);

            var userIndexKeys = Builders<User>.IndexKeys
               .Ascending(x => x.Email)
               .Ascending(x => x.RefreshToken);
            CreateIndex(userIndexKeys);

            var emailSubscriberIndexKeys = Builders<EmailSubscriber>.IndexKeys
                .Ascending(x => x.Email);
            CreateIndex(emailSubscriberIndexKeys);

            var newsletterIndexKeys = Builders<Newsletter>.IndexKeys
                .Ascending(x => x.SentAt);
            CreateIndex(newsletterIndexKeys);
        }

        private void CreateIndex<T>(IndexKeysDefinition<T> indexKeys, bool unique = false)
        {
            var collection = _client.GetDatabase(_databaseName).GetCollection<T>(GetCollectionName<T>());
            var options = new CreateIndexOptions { Unique = unique, Background = true };
            collection.Indexes.CreateOne(new CreateIndexModel<T>(indexKeys, options));
        }
        private static string GetCollectionName<T>() => typeof(T).Name + "s";
    }
}
