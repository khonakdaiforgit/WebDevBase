using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyApp.Domain.Dtos;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces;
using MyApp.Infrastructure.Repositories.Common;


namespace MyApp.Infrastructure.Repositories 
{

    public class LogRepository : MongoRepository<LogEntry>, ILogRepository
    {
        public LogRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
        {
        }

        public async Task InsertLogAsync(LogEntry logEntry)
        {
            await _collection.InsertOneAsync(logEntry);
        }

        public async Task<IEnumerable<LogEntry>> GetAllLogsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByUserAsync(string userId)
        {
            return await _collection.Find(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogsByLevelAsync(string level)
        {
            return await _collection.Find(x => x.Level == level).ToListAsync();
        }

        public IEnumerable<LogEntry> GetFilteredLogsAsync(FilterDefinition<LogEntry> filter)
        {
            return _collection.Find(filter).ToEnumerable();
        }

        public async Task<PagedLogResult> GetFilteredLogsWithPagingAsync(FilterDefinition<LogEntry> filter, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Min(Math.Max(1, pageSize), 100); // محدود کردن حداکثر PageSize

            var countTask = _collection.CountDocumentsAsync(filter);
            var logsTask = _collection
                .Find(filter)
                .SortByDescending(c => c.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            await Task.WhenAll(countTask, logsTask);

            return new PagedLogResult
            {
                Logs = logsTask.Result,
                TotalCount = (int)countTask.Result,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}