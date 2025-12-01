using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Application.Common;

namespace MyApp.Application.Abstractions.Users
{
    public interface IUserService
    {
        Task<UserDto?> GetCurrentUserAsync(Guid userId);
        Task DeactivateAsync(Guid targetUserId);
        Task ActivateAsync(Guid targetUserId);
        Task<PagedResult<UserDto>> GetListAsync(int page = 1, int pageSize = 20);
    }
}