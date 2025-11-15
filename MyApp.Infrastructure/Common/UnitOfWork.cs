using MongoDB.Driver;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories;
using MyApp.Infrastructure.Repositories.Interface;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MongoDbContext _context;
    private IClientSessionHandle? _session;
    private bool _disposed = false;

    public UnitOfWork(MongoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        Users = new UserRepository(_context);
        Restaurants = new RestaurantRepository(_context);
        MenuCategories = new MenuCategoryRepository(_context);
        MenuItems = new MenuItemRepository(_context);
        GalleryItems = new GalleryItemRepository(_context);
        News = new NewsRepository(_context);
        Newsletters = new NewsletterRepository(_context);
        EmailSubscribers = new EmailSubscriberRepository(_context);
        ContactMessages = new ContactMessageRepository(_context);
        Logs = new LogEntryRepository(_context);
    }

    // ریپازیتوری‌ها...
    public IUserRepository Users { get; }
    public IRestaurantRepository Restaurants { get; }
    public ILogEntryRepository Logs { get; }
    public IMenuCategoryRepository MenuCategories { get; }
    public IMenuItemRepository MenuItems { get; }
    public IGalleryItemRepository GalleryItems { get; }
    public INewsRepository News { get; }
    public INewsletterRepository Newsletters { get; }
    public IEmailSubscriberRepository EmailSubscribers { get; }
    public IContactMessageRepository ContactMessages { get; }

    // --- تراکنش با Session ---
    public async Task<IClientSessionHandle> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_session == null)
        {
            _session = await _context.Client.StartSessionAsync(cancellationToken: ct);
            _session.StartTransaction();
        }
        return _session;
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_session?.IsInTransaction == true)
        {
            await _session.CommitTransactionAsync(ct);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_session?.IsInTransaction == true)
        {
            await _session.AbortTransactionAsync(ct);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // در MongoDB، SaveChanges برای Session معنی داره
        return Task.FromResult(1);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _session?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}