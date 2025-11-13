using MyApp.Application.Common;
using MyApp.Domain.Interfaces.Common;
using System.Linq.Expressions;


namespace MyApp.Infrastructure.Repositories.Interface.Common
{
    /// <summary>
    /// ریپازیتوری عمومی برای کار با MongoDB
    /// فقط برای Entityهایی که IHasId<Guid> دارند
    /// </summary>
    public interface IGenericRepository<T> where T : class, IHasId<Guid>
    {
        /// <summary>
        /// دریافت موجودیت بر اساس Id
        /// </summary>
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// دریافت تمام موجودیت‌ها (با یا بدون فیلتر)
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            CancellationToken ct = default);

        /// <summary>
        /// صفحه‌بندی + فیلتر + مرتب‌سازی
        /// </summary>
        Task<PagedResult<T>> GetPagedAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool orderByDescending = false,
            int page = 1,
            int pageSize = 20,
            CancellationToken ct = default);

        /// <summary>
        /// افزودن موجودیت جدید
        /// </summary>
        Task AddAsync(T entity, CancellationToken ct = default);

        /// <summary>
        /// افزودن چندتایی
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        /// <summary>
        /// به‌روزرسانی موجودیت (جایگزینی کامل)
        /// </summary>
        Task UpdateAsync(T entity, CancellationToken ct = default);

        /// <summary>
        /// حذف منطقی یا فیزیکی
        /// </summary>
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// بررسی وجود
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);

        /// <summary>
        /// شمارش با فیلتر
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
    }
}
