namespace MyApp.Application.Abstractions.Authorization
{
    /// <summary>
    /// نقش‌های مجاز برای هر عملیات
    /// </summary>
    public enum AccessLevel
    {
        Owner,      // مالک پروژه (فقط شما)
        StoreAdmin  // مدیر فروشگاه (Restaurant Owner)
    }
}