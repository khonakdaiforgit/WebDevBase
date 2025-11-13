using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;

namespace MyApp.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client; // اضافه شد!

    public MongoDbContext(string connectionString, string databaseName)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        //settings.GuidRepresentation = GuidRepresentation.Standard;
        _client = new MongoClient(settings);
        _database = _client.GetDatabase(databaseName);
    }

    public MongoDbContext(IMongoClient client, string databaseName)
    {
        _client = client;
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>() where T : class, IHasId<Guid>
        => _database.GetCollection<T>(typeof(T).Name + "s");

    public IMongoCollection<LogEntry> Logs => _database.GetCollection<LogEntry>("LogEntries");

    // اضافه شد: دسترسی به Client برای Session
    public IMongoClient Client => _client;
}