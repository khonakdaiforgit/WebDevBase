using MyApp.Application.Abstractions.Authorization;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;

namespace MyApp.Infrastructure.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task DeactivateAsync(Guid targetUserId)
        {
 
            var user = await _unitOfWork.Users.GetByIdAsync(targetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            user.Deactivate();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(Guid targetUserId)
        {

            var user = await _unitOfWork.Users.GetByIdAsync(targetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            user.Activate();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<UserDto>> GetListAsync(int page = 1, int pageSize = 20)
        {
            var skip = (page - 1) * pageSize;

            var users = await _unitOfWork.Users.GetPagedAsync(
                filter: null,
                orderBy: u => u.CreatedAt,
                orderByDescending: true,
                page: page,
                pageSize: pageSize);

            var dtos = users.Items.Select(MapToDto).ToList();

            return new PagedResult<UserDto>(dtos, users.TotalCount, page, pageSize);
        }

        private UserDto MapToDto(User user) => new(
            Id: user.Id,
            Email: user.Email,
            Role: user.Role,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt);
    }
}
