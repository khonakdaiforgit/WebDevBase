namespace MyApp.Application.Abstractions.Authorization
{
    public interface IAuthorizationService
    {
        Task<bool> CanAccessRestaurantAsync(Guid userId, AccessLevel required = AccessLevel.StoreAdmin);
        Task<bool> CanViewLogsAsync(Guid userId); // فقط Owner
        Task<bool> IsOwnerAsync(Guid userId);
    }
}