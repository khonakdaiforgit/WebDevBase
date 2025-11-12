namespace MyApp.Domain.Interfaces.Common
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
