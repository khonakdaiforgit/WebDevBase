// src/Application/Common/IUnitOfWork.cs
using MongoDB.Driver;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Common;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRestaurantRepository Restaurants { get; }
    ILogEntryRepository Logs { get; }
    IMenuCategoryRepository MenuCategories { get; }
    IMenuItemRepository MenuItems { get; }
    IGalleryItemRepository GalleryItems { get; }
    INewsRepository News { get; }
    INewsletterRepository Newsletters { get; }
    IEmailSubscriberRepository EmailSubscribers { get; }
    IContactMessageRepository ContactMessages { get; }

    Task<IClientSessionHandle> BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}