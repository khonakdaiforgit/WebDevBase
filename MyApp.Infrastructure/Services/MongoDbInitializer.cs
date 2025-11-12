using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyApp.Application.UseCases.User;

namespace MyApp.Infrastructure.Services
{
    public class MongoDbInitializer : IHostedService
    {
        private readonly MongoMapping _mapping;
        private readonly MongoIndexing _indexing;
        private readonly IServiceScopeFactory _scopeFactory;

        public MongoDbInitializer(
            MongoMapping mapping,
            MongoIndexing indexing,
            IServiceScopeFactory scopeFactory)
        {
            _mapping = mapping;
            _indexing = indexing;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _mapping.RegisterMappings();
            _indexing.RegisterIndexing();

            // ایجاد Scope جدید برای UseCase
            using var scope = _scopeFactory.CreateScope();
            var seedAdmin = scope.ServiceProvider.GetRequiredService<SeedAdminUseCase>();
            await seedAdmin.ExecuteAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
