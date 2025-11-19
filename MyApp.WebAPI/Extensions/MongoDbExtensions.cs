using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Services.Email.Settings;

namespace MyApp.WebAPI.Extensions
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services.Configure<SmtpSettings>(config.GetSection("Smtp"));
            services.Configure<AppSettings>(config.GetSection("App"));

            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(MongoClientSettings.FromConnectionString(
                    config.GetConnectionString("MongoDb")!)));

            services.AddSingleton<MongoDbContext>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return new MongoDbContext(client, "RestaurantAppDb");
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
