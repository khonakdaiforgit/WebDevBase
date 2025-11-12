using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Common;
using System.Linq.Expressions;

namespace MyApp.Infrastructure
{
    public class MongoMapping
    {
      
        public void RegisterMappings()
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));

            Register<User>();
            Register<EmailSubscriber>();
            Register<Newsletter>();
            Register<Restaurant>();
            Register<MenuCategory>();
            Register<MenuItem>();
            Register<News>();
            Register<GalleryItem>();
            Register<ContactMessage>();
            Register<LogEntry>();
        }

        private void Register<T>(Expression<Func<T, object>>? indexField = null) where T : class
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(T))) return;

            var cm = new BsonClassMap<T>();
            cm.AutoMap();
            cm.MapIdProperty(x => ((IHasId<Guid>)x).Id)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

            BsonClassMap.RegisterClassMap(cm);
        }
      
    }
}
