using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Seed;

namespace MyApp.WebAPI.Extensions
{
    public static class ApplicationExtensions
    {
        public static async Task SeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await SeedData.InitializeAsync(uow);
        }
    }
}
